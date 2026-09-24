using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VideoStreamingDownloader.Common.Downloads
{
    internal abstract class Downloader
    {
        internal Status Status { get; private set; }
        internal abstract Task Download();
        internal string FileId { get; private set; }

        protected Downloader(int maxStatusValue, string fileId)
        {
            Status = new Status(maxStatusValue);
            FileId = fileId;
        }

        protected async Task<string> DownloadParts(Urls Urls, string folder, bool showProgress = true)
        {
            int segmentCount = Urls.SegmentStart;
            if (showProgress)
                Status.SetProgressValue(segmentCount);

            using (var file = File.Create($"{folder}\\{Guid.NewGuid()}.mp4", 4 * 1024, FileOptions.WriteThrough))
            {
                var init = await HttpUtils.GetRequestByteArrayResponse(Urls.InitUrl);
                file.Write(init, 0, init.Length);

                var dataPart = await HttpUtils.GetRequestByteArrayResponse(string.Format(Urls.DataUrl, segmentCount));

                while (dataPart != null)
                {
                    file.Write(dataPart, 0, dataPart.Length);
                    segmentCount++;
                    if (showProgress)
                        Status.SetProgressValue(segmentCount);
                    dataPart = await HttpUtils.GetRequestByteArrayResponse(string.Format(Urls.DataUrl, segmentCount));
                }

                return file.Name;
            }
        }

        protected async Task<string> DirectDownload(string url, string folder)
        {
            string path = $"{folder}\\{Guid.NewGuid()}.mp4";

            using (WebClient web = new WebClient())
            {
                web.DownloadProgressChanged += (s, e) => { Status.SetProgressValue(e.ProgressPercentage); };
                await web.DownloadFileTaskAsync(url, path);
            }

            return path;
        }

        protected string RemoveVttStyles(string text)
        {
            StringBuilder stringBuilder = new StringBuilder("WEBVTT");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(text.Substring(text.IndexOf("1\r\n")));

            return stringBuilder.ToString();
        }
    }
}
