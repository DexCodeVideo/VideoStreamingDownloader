using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VideoStreamingDownloader._3CAT.Media;
using VideoStreamingDownloader.Common;
using VideoStreamingDownloader.Common.Downloads;

namespace VideoStreamingDownloader._3CAT.Downloads
{
    internal class Downloader : Common.Downloads.Downloader
    {
        private readonly string OriginDescription = "3CAT";
        private Item _downloadOptions;
        private _3CATService _3CatService;

        internal Downloader(Item downloadOptions) :
            base(CalculateMaxStatusValue(downloadOptions), downloadOptions.Id)
        {
            _downloadOptions = downloadOptions;
            _3CatService = new _3CATService(int.Parse(_downloadOptions.FileInfo.Id));
        }

        private static int CalculateMaxStatusValue(Item downloadOptions)
        {
            return downloadOptions.Video.DirectLink == string.Empty
                    ? downloadOptions.FileInfo.Duration > 0
                        ? downloadOptions.FileInfo.Duration / 4000 + 5
                        : 1000
                    : 100;
        }

        private void LogInformation(string message)
        {
            Program.LoggerService.LogInformation($"[3CAT][{_downloadOptions.FileInfo.Id}] {message}");
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
                MergeFile(results, filename);
            }
            catch (Exception ex)
            {
                Program.LoggerService.LogError(ex);
                throw;
            }
            finally
            {
                Program.LoggerService.LogInformation($"Removing folder: {folder}");
                Directory.Delete(folder, true);
            }
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
            string path;
            if (video.DirectLink == string.Empty)
                path = await DownloadParts(video.Urls, baseDownloadPath);
            else
                path = await DirectDownload(video.DirectLink, baseDownloadPath);

            return new Result<IMetadata>(new VideoMetada(), path);
        }

        internal async Task<Result<IMetadata>> AudioDownloadTask(Track.Video video, Track.Audio audio, string baseDownloadPath, bool showProgress)
        {
            if (video.HasVideo && video.DirectLink != string.Empty)
                return await EmptyAudioTask();

            string path;
            if (audio.DirectLink == string.Empty)
                path = await DownloadParts(audio.Urls, baseDownloadPath, showProgress);
            else
                path = await DirectDownload(audio.DirectLink, baseDownloadPath);

            return new Result<IMetadata>(new AudioMetada(audio.Language.IsoCode), path);
        }

        private Task<Result<IMetadata>> EmptyAudioTask()
        {
            return Task.FromResult(new Result<IMetadata>(new AudioMetada(_downloadOptions.Audios.First().Language.IsoCode), string.Empty));
        }

        internal async Task<Result<IMetadata>> SubtitleDownloadTask(Track.Subtitle subtitle, string baseDownloadPath)
        {
            string downloadPath = $"{baseDownloadPath}\\{Guid.NewGuid()}.{subtitle.Format}";
            string vttSubs = await HttpUtils.GetRequestStringResponse(subtitle.DirectLink);
            File.WriteAllText(downloadPath, RemoveVttStyles(vttSubs));
            return new Result<IMetadata>(new SubtitleMetada(subtitle.Language.IsoCode), downloadPath);
        }

        private async Task<(Track.Video, Tracks.Audio, Tracks.Subtitle)> GetMediaOptions()
        {
            Status.SetMessage(Resources.Translations.Strings.Getting_media);
            LogInformation("Obtaining media options");

            var options = await _3CatService.GetMediaOptions();

            Track.Video video = ObtainVideoTrack(options.Videos);
            Tracks.Audio audios = new Tracks.Audio(options.Audios.Where(audio => _downloadOptions.Audios.Any(x => x.Language.IsoCode == audio.Language.IsoCode)));
            Tracks.Subtitle subtitles = new Tracks.Subtitle(options.Subtitles.Where(subtitle => _downloadOptions.Subtitles.Any(x => x.Language.IsoCode == subtitle.Language.IsoCode)));

            LogInformation("Obtaining media options completed");
            Status.Itereate();
            return (video, audios, subtitles);
        }

        private Track.Video ObtainVideoTrack(Tracks.Video videos)
        {
            Track.Video video = videos.Where(x => x.Resolution == _downloadOptions.Video.Resolution).FirstOrDefault();
            if (video != null)
                return video;

            Program.LoggerService.LogWarning($"Video resolution {_downloadOptions.Video.Resolution} not found for video {_downloadOptions.FileInfo.Title}[{_downloadOptions.FileInfo.Id}]{Environment.NewLine}" +
                $"\tFinding best next video quality");

            return ObtainBestVideoTrack(videos);
        }

        private Track.Video ObtainBestVideoTrack(Tracks.Video videos)
        {
            return videos.OrderByDescending(value =>
            {
                if (!value.HasVideo)
                    return -1;

                return int.TryParse(value.Resolution.TrimEnd('p'), out int quality)
                    ? quality
                    : -1;
            }).First();
        }

        internal async Task<Results> DownloadTracks(Track.Video video, Tracks.Audio audios, Tracks.Subtitle subtitles, string tempFolder)
        {
            Status.SetMessage(Resources.Translations.Strings.Downloading);
            LogInformation("Download started");

            List<Task<Result<IMetadata>>> downloadTasks = new List<Task<Result<IMetadata>>>();

            bool HasVideo = video.HasVideo;
            bool ShowAudioProgress = !HasVideo;

            if (HasVideo)
                downloadTasks.Add(VideoDownloadTask(video, tempFolder));

            foreach (var audio in audios)
            {
                downloadTasks.Add(AudioDownloadTask(video, audio, tempFolder, ShowAudioProgress));
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
