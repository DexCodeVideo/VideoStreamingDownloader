using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace VideoStreamingDownloader.Common.Downloads
{
    internal class Data
    {
        public Items DownloadItems { get; set; } = new Items();
        public List<Item> ItemsHistoric { get; set; } = new List<Item>();
    }

    internal class Manager
    {
        private static readonly string _downloaderFile = Path.Combine(Program.UserDataDirectory, "Downloader.json");
        internal IndexedConcurrentQueue DownloadsQueue { get; private set; } = new IndexedConcurrentQueue();
        internal List<Downloader> ActiveDownloads { get; private set; } = new List<Downloader>();
        internal Data DownloadData { get; set; } = new Data();

        internal bool HaveDownloadsPending => DownloadData.DownloadItems.Any(x => x.Value.State == 0);
        internal bool DownloadEnable => HaveDownloadsPending && !IsDownloading;
        internal bool IsDownloading { get; private set; } = false;
        internal int FinishedDownloads { get; private set; } = 0;
        internal int TotalDownloads { get; private set; } = 0;
        internal int DownloadSize => DownloadsQueue.Count;

        internal static Manager Get()
        {
            if (!File.Exists(_downloaderFile))
                return new Manager();

            try
            {
                string data = File.ReadAllText(_downloaderFile);
                return Get(JsonSerializer.Deserialize<Data>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }));
            }
            catch (Exception ex)
            {
                Program.LoggerService.LogError("DownloadManger parser failed", ex);
                return new Manager();
            }
        }

        internal static Manager Get(Data downloadData)
        {
            return new Manager()
            {
                DownloadData = downloadData
            };
        }

        internal void BatchAdd(IEnumerable<Item> downloadItem)
        {
            bool shouldSave = false;

            foreach (var item in downloadItem)
            {
                if (!DownloadData.DownloadItems.ContainsKey(item.Id))
                {
                    shouldSave = true;
                    DownloadData.DownloadItems[item.Id] = item;
                    var downloader = GetDownloader(item);
                    if (DownloadsQueue.TryPush(downloader))
                        TotalDownloads++;
                }
            }

            if (shouldSave)
            {
                Save();
                LoadDownloadsCue();
            }
        }

        private readonly object _saveLock = new object();

        private void Save()
        {
            lock (_saveLock)
            {
                var json = JsonSerializer.Serialize(
                    DownloadData,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });

                File.WriteAllText(_downloaderFile, JsonSerializer.Serialize(DownloadData));
            }
        }

        CancellationTokenSource DownloadCancellationTokenSource = new CancellationTokenSource();

        internal async Task StartDownload()
        {
            if (!IsDownloading)
            {
                IsDownloading = true;
                LoadDownloadsCue();

                List<Task> workers = new List<Task>();

                for (int i = 0; i < Program.UserSettings.SimultaneousDownloads; i++)
                {
                    workers.Add(Download(DownloadCancellationTokenSource.Token));
                }

                await Task.WhenAll(workers);
                IsDownloading = false;
            }
        }

        private async Task Download(CancellationToken token)
        {
            while (!token.IsCancellationRequested && DownloadsQueue.TryPop(out var downloader))
            {
                await Task.Run(() => ProcessSingleDownload(downloader));
                FinishedDownloads++;
            }
        }

        private async Task ProcessSingleDownload(Downloader downloader)
        {
            ActiveDownloads.Add(downloader);
            var item = DownloadData.DownloadItems[downloader.FileId];
            item.StartedDate = DateTime.Now;
            item.State = Item.States.Downloading;
            Save();
            try
            {
                await downloader.Download();
                item.State = Item.States.Complete;
                item.EndDate = DateTime.Now;
            }
            catch (Exception ex)
            {
                item.ErrorMessage = ex.Message;
                item.State = Item.States.Error;
            }
            finally
            {
                ActiveDownloads.Remove(downloader);
                Save();
            }
        }

        internal void FirstLoadDownloadsCue()
        {
            List<string> ItemsToRemove = new List<string>();

            foreach (var downloadItem in DownloadData.DownloadItems)
            {
                var downloadItemValue = downloadItem.Value;

                switch (downloadItemValue.State)
                {
                    case Item.States.Pending:
                        DownloadsQueue.TryPush(GetDownloader(downloadItemValue));
                        break;

                    case Item.States.Downloading:
                        AddHaltedToHistory(downloadItemValue);
                        RetryIfPossible(downloadItem, ItemsToRemove);
                        break;

                    case Item.States.Complete:
                        DownloadData.ItemsHistoric.Add(downloadItemValue);
                        ItemsToRemove.Add(downloadItem.Key);
                        break;

                    case Item.States.Error:
                        DownloadData.ItemsHistoric.Add(downloadItemValue.Clone());
                        RetryIfPossible(downloadItem, ItemsToRemove);
                        break;
                }
            }

            RemoveItems(ItemsToRemove);
            Save();
            TotalDownloads = DownloadsQueue.Count;
        }

        private void AddHaltedToHistory(Item item)
        {
            var historicItem = item.Clone();
            historicItem.State = Item.States.Error;
            historicItem.ErrorMessage = "Download halted";

            DownloadData.ItemsHistoric.Add(historicItem);
        }

        private void RemoveItems(List<string> itemsToRemove)
        {
            foreach (string itemToRemove in itemsToRemove)
            {
                DownloadData.DownloadItems.Remove(itemToRemove);
            }
        }

        private void RetryIfPossible(KeyValuePair<string, Item> downloadItem, List<string> itemsToRemove)
        {
            var downloadItemValue = downloadItem.Value;
            downloadItemValue.State = Item.States.Pending;
            downloadItemValue.ErrorMessage = "";
            downloadItemValue.RetryCount++;
            if (downloadItemValue.RetryCount > 5)
                itemsToRemove.Add(downloadItem.Key);
            else
                DownloadsQueue.TryPush(GetDownloader(downloadItemValue));
        }


        internal void LoadDownloadsCue()
        {
            foreach (var downloadItem in DownloadData.DownloadItems.Where(x => x.Value.State == 0))
            {
                var downloader = GetDownloader(downloadItem.Value);
                if (DownloadsQueue.TryPush(downloader))
                    TotalDownloads++;
            }
        }

        private Downloader GetDownloader(Item downloadItem)
        {
            switch (downloadItem)
            {
                case _3CAT.Downloads.Item _3CATDownloadOptions:
                    return new _3CAT.Downloads.Downloader(_3CATDownloadOptions);
                case RTVE.Downloads.Item RTVEDownloadOptions:
                    return new RTVE.Downloads.Downloader(RTVEDownloadOptions);
                case ETB.Downloads.Item ETBDownloadOptions:
                    return new ETB.Downloads.Downloader(ETBDownloadOptions);
                default:
                    throw new NotImplementedException();
            }
        }

        internal void Abort()
        {
            foreach (var downloadItem in DownloadData.DownloadItems.Where(x => x.Value.State == 1))
            {
                downloadItem.Value.ErrorMessage = "Download aborted";
                downloadItem.Value.State = -1;
            }

            Save();
        }
    }
}
