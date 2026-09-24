using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using VideoStreamingDownloader.Common;
using VideoStreamingDownloader.Common.Downloads;
using VideoStreamingDownloader.RTVE.DTOs.Response;
using VideoStreamingDownloader.RTVE.Media;
using VideoStreamingDownloader.RTVE.Response;

namespace VideoStreamingDownloader.RTVE
{
    internal class RTVEService
    {
        private const string InfoUrl = "https://api-ztnr.rtve.es/api/videos/{0}.json";
        private const string TokenUrl = "https://api.rtve.es/api/token/{0}";
        private const string MpdUrl = "https://ztnr.rtve.es/ztnr/{0}.mpd";

        internal int _id;
        private int _mediaType = MediaTypeCodes.None;

        internal RTVEService() { }

        internal RTVEService(int id)
        {
            _id = id;
            _mediaType = MediaTypeCodes.Single;
        }

        internal async Task<int> IdentifyUrl(string url)
        {
            int score = 0;

            if (url.ToLower().Contains("rtve"))
                score++;

            try
            {
                _id = GetEpisodeCode(url);
                if (!await IsValidEpisode(_id))
                    throw new Exception("");
                _mediaType = MediaTypeCodes.Single;
                score++;
            }
            catch { }

            return score;
        }

        private async Task<bool> IsValidEpisode(int code)
        {
            return await GetWidevineUrl(code) != string.Empty;
        }

        private int GetEpisodeCode(string url)
        {
            string[] splitedUrl = url.Split('/');
            int code;

            if (!int.TryParse(splitedUrl[splitedUrl.Length - 1], out code))
                if (!int.TryParse(splitedUrl[splitedUrl.Length - 2], out code))
                    throw new Exception("Episode not found");

            return code;
        }

        internal async Task<Media.Options> GetMediaOptions()
        {
            var (videos, audios, subs) = await GetTracks(_id);
            var mediaResponse = await GetMediaResponse(_id);

            return new Media.Options
            {
                FileInfos = new FileInfos() { FileInfo.Create(mediaResponse) },
                Videos = videos,
                Audios = audios,
                Subtitles = subs,
            };
        }

        internal async Task<string> GetDecryptionKey(Track.Video video)
        {
            var widevineUrl = await GetWidevineUrl(_id);
            if (string.IsNullOrEmpty(widevineUrl))
                Program.LoggerService.LogError("Token failed");
            return Utils.ObtainDecryptiontKey(widevineUrl, video.Pssh);
        }

        private async Task<string> GetWidevineUrl(int episodeCode)
        {
            TokenResponse response = await HttpUtils.GetRequestDeserializedResponse<TokenResponse>(string.Format(TokenUrl, episodeCode));
            return response.WidevineUrl;
        }

        private async Task<MediaResponse> GetMediaResponse(int episodeCode)
        {
            string url = string.Format(InfoUrl, episodeCode);
            return await HttpUtils.GetRequestDeserializedResponse<MediaResponse>(url);
        }

        private async Task<(Tracks.Video, Tracks.Audio, Tracks.Subtitle)> GetTracks(int code)
        {
            string mpdLink = string.Format(MpdUrl, code);

            string mpdFile = await HttpUtils.GetRequestStringResponse(mpdLink);

            XDocument doc = XDocument.Parse(mpdFile);

            var res = doc.Descendants(doc.Root.Name.Namespace + "AdaptationSet");

            var baseVideo = res.Where(x => x.Attribute("contentType").Value == "video");
            var baseAudio = res.Where(x => x.Attribute("contentType").Value == "audio");
            var baseSubtitles = res.Where(x => x.Attribute("contentType").Value == "text");

            var pssh = GetPssh(baseVideo.Descendants(doc.Root.Name.Namespace + "ContentProtection"));

            return
            (
                Tracks.Video.Create(baseVideo.Descendants(doc.Root.Name.Namespace + "Representation"), pssh),
                Tracks.Audio.Create(doc, baseAudio),
                Tracks.Subtitle.Create(doc, baseSubtitles)
            );
        }

        private string GetPssh(IEnumerable<XElement> docs)
        {
            foreach (XElement element in docs)
            {
                if (element.FirstNode is XElement node)
                    return node.Value;
            }

            return string.Empty;
        }

        private class MediaTypeCodes
        {
            internal const int None = 0;
            internal const int Single = 1;
        }
    }
}
