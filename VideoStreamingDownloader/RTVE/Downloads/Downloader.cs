using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoStreamingDownloader.Common.Downloads;
using VideoStreamingDownloader.RTVE.Media;

namespace VideoStreamingDownloader.RTVE.Downloads
{
    internal class Downloader : Common.Downloads.Downloader
    {
        private readonly string OriginDescription = "RTVE";
        private Item _downloadOptions;
        private RTVEService _RTVEService;

        internal Downloader(Item downloadOptions) :
            base(CalculateMaxStatusValue(downloadOptions), downloadOptions.Id)
        {
            _downloadOptions = downloadOptions;
            _RTVEService = new RTVEService(int.Parse(_downloadOptions.FileInfo.Id));
        }

        private static int CalculateMaxStatusValue(Item downloadOptions)
        {
            return downloadOptions.FileInfo.Duration / 5000 + 10;
        }

        private void LogInformation(string message)
        {
            Program.LoggerService.LogInformation($"[RTVE][{_downloadOptions.FileInfo.Id}] {message}");
        }

        internal override async Task Download()
        {
            string filename = Common.Downloads.Utils.CreateFilename(_downloadOptions.DestinationPath, _downloadOptions.FileInfo.Title);
            if (File.Exists(filename))
                return;

            string folder = Common.Downloads.Utils.GetTempFolder(OriginDescription, _downloadOptions.FileInfo.Id);
            Common.Downloads.Utils.CreateDirectory(folder);

            try
            {
                var (video, audios, subtitles) = await GetMediaOptions();
                var results = await DownloadTracks(video, audios, subtitles, folder);
                var dKey = await GetDecryptionKey(video);
                await DecryptResults(results, dKey);
                MergeFile(results, filename);
            }
            catch (Exception ex)
            {
                Program.LoggerService.LogError(ex);
                throw;
            }
            finally
            {
                LogInformation($"Removing folder: {folder}");
                Directory.Delete(folder, true);
            }
        }

        private async Task<string> GetDecryptionKey(Track.Video video)
        {
            Status.SetMessage(Resources.Translations.Strings.Getting_key);
            LogInformation("Obtaining decryption key started");
            string key = await _RTVEService.GetDecryptionKey(video);
            LogInformation("Obtaining decryptiont key completed");
            Status.Itereate();
            return key;
        }

        private async Task DecryptResults(Results results, string decryptKey)
        {
            Status.SetMessage(Resources.Translations.Strings.Decripting);
            LogInformation("Results decryption started");
            var resultsToDecrypt = results.Where(result => result.Metadata.GetType() != typeof(SubtitleMetada));
            await Task.WhenAll(resultsToDecrypt.Select(result => Task.Run(() => Utils.DecryptFile(result.Path, decryptKey))));
            Status.Itereate();
            LogInformation("Results decryption completed");
        }

        private void MergeFile(Results results, string filename)
        {
            Status.SetMessage(Resources.Translations.Strings.Merging_files);
            LogInformation("Merging started");
            Common.Downloads.Utils.MergeFile(results, filename);
            LogInformation("Merging completed");
        }

        internal async Task<Result<IMetadata>> VideoDownloadTask(Track.Video video, string baseDownloadPath)
        {
            var path = await DownloadParts(video.Urls, baseDownloadPath);
            return new Result<IMetadata>(new VideoMetada(), path);
        }

        internal async Task<Result<IMetadata>> AudioDownloadTask(Track.Audio audio, string baseDownloadPath, bool showProgress)
        {
            string path = await DownloadParts(audio.Urls, baseDownloadPath, showProgress);
            return new Result<IMetadata>(new AudioMetada(audio.Language.IsoCode), path);
        }

        internal async Task<Result<IMetadata>> SubtitleDownloadTask(Track.Subtitle subtitle, string baseDownloadPath)
        {
            string tempPath = await DownloadParts(subtitle.Urls, baseDownloadPath, false);
            string path = Common.Downloads.Utils.ExtractVtt(tempPath);
            return new Result<IMetadata>(new SubtitleMetada(subtitle.Language.IsoCode), path);
        }

        private async Task<(Track.Video, Tracks.Audio, Tracks.Subtitle)> GetMediaOptions()
        {
            Status.SetMessage(Resources.Translations.Strings.Getting_media);
            LogInformation("Obtaining media options");

            var options = await _RTVEService.GetMediaOptions();

            Track.Video video = options.Videos.Where(x => x.Resolution == _downloadOptions.Video.Resolution).FirstOrDefault();
            Tracks.Audio audios = new Tracks.Audio(options.Audios.Where(audio => _downloadOptions.Audios.Any(x => x.Language.IsoCode == audio.Language.IsoCode)));
            Tracks.Subtitle subtitles = new Tracks.Subtitle(options.Subtitles.Where(subtitle => _downloadOptions.Subtitles.Any(x => x.Language.IsoCode == subtitle.Language.IsoCode)));

            LogInformation("Obtaining media options completed");
            Status.Itereate();
            return (video, audios, subtitles);
        }

        internal async Task<Results> DownloadTracks(Track.Video video, Tracks.Audio audios, Tracks.Subtitle subtitles, string tempFolder)
        {
            Status.SetMessage(Resources.Translations.Strings.Downloading);
            LogInformation("Download started");

            List<Task<Result<IMetadata>>> downloadTasks = new List<Task<Result<IMetadata>>>();

            bool ShowAudioProgress = !video.HasVideo;

            if (video.HasVideo)
                downloadTasks.Add(VideoDownloadTask(video, tempFolder));

            foreach (var audio in audios)
            {
                downloadTasks.Add(AudioDownloadTask(audio, tempFolder, ShowAudioProgress));
                ShowAudioProgress = false;
            }

            foreach (var subtitle in subtitles)
            {
                downloadTasks.Add(SubtitleDownloadTask(subtitle, tempFolder));
            }

            await Task.WhenAll(downloadTasks);

            Results results = new Results();

            foreach (var downloadTask in downloadTasks)
            {
                results.Add(await downloadTask);
            }

            LogInformation("Download completed");

            return results;
        }
    }
}
