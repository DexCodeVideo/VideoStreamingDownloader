using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using VideoStreamingDownloader.Common;
using VideoStreamingDownloader.Common.Downloads;
using VideoStreamingDownloader.ETB.DTOs.Response;
using VideoStreamingDownloader.ETB.Media;

namespace VideoStreamingDownloader.ETB
{
    internal class ETBService
    {
        private const string InfoUrl = "https://etbon.eus/api/v1/media/{0}";
        private const string MultipleInfoUrl = "https://etbon.eus/api/v1/series/{0}";
        private const string BaseUrl = "https://etbon.eus/";
        private const string MpdUrl = "https://etbon.eus/manifests/{0}/eu/widevine/dash.mpd";

        internal string _id;
        private int _mediaType = MediaTypeCodes.None;

        internal ETBService() { }

        internal ETBService(string id)
        {
            _id = id;
            _mediaType = MediaTypeCodes.Single;
        }

        internal async Task<int> IdentifyUrl(string url)
        {
            int score = 0;

            if (url.ToLower().Contains("etb"))
                score++;

            try
            {
                _id = GetId(url);
                if (!string.IsNullOrEmpty(_id))
                {
                    if (!await IsValidEpisode(_id))
                        throw new Exception("");
                    _mediaType = MediaTypeCodes.Single;
                    score++;
                }
            }
            catch { }
            try
            {
                _id = await GetProgramId(url);
                if (!string.IsNullOrEmpty(_id))
                {
                    _mediaType = MediaTypeCodes.Multiple;
                    score++;
                }
            }
            catch { }

            return _mediaType == MediaTypeCodes.None ? 0 : score;
        }

        private async Task<bool> IsValidEpisode(string id)
        {
            return await GetWidevineUrl(id) != string.Empty;
        }

        private string GetId(string url)
        {
            string[] splitedUrl = url.Split('/');
            return splitedUrl.Last();
        }

        private async Task<string> GetProgramId(string url)
        {
            string id = GetId(url);
            var res = await GetMultipleMediaResponse(_id);
            return id;
        }

        internal async Task<Media.Options> GetMediaOptions()
        {
            string id = _id;
            MultipleMediaResponse multipleMediaResponse = null;
            FileInfos fileInfos = new FileInfos();

            if (_mediaType == MediaTypeCodes.Multiple)
            {
                multipleMediaResponse = await GetMultipleMediaResponse(_id);
                id = multipleMediaResponse.Seasons.First().Episodes.First().Slug;
                fileInfos = FileInfos.Create(multipleMediaResponse);
            }

            MediaResponse singleMediaResponse = await GetMediaResponse(id);

            if (fileInfos.Count == 0)
                fileInfos.Add(FileInfo.Create(singleMediaResponse));

            var (videos, audios, subtitles) = await GetTracks(singleMediaResponse.Slug);

            return new Media.Options()
            {
                Videos = videos,
                Audios = audios,
                Subtitles = subtitles,
                FileInfos = fileInfos,
            };
        }

        internal async Task<string> GetDecryptionKey(Track.Video video)
        {
            var widevineUrl = await GetWidevineUrl(_id);
            if (string.IsNullOrEmpty(widevineUrl))
                Program.LoggerService.LogError("Token failed");
            return Utils.ObtainDecryptiontKey(BaseUrl + widevineUrl, video.Pssh);
        }

        private async Task<string> GetWidevineUrl(string id)
        {
            MediaResponse mediaResponse = await GetMediaResponse(id);
            return mediaResponse.Manifests.Where(x => x.DrmConfig.Type == "widevine").Select(x => x.DrmConfig.licenseAcquisitionURL).FirstOrDefault();
        }

        private async Task<MediaResponse> GetMediaResponse(string id)
        {
            string url = string.Format(InfoUrl, id);
            return await HttpUtils.GetRequestDeserializedResponse<MediaResponse>(url, new Dictionary<string, string> { { "lang", "es" } });
        }

        private async Task<MultipleMediaResponse> GetMultipleMediaResponse(string id)
        {
            string url = string.Format(MultipleInfoUrl, id);
            return await HttpUtils.GetRequestDeserializedResponse<MultipleMediaResponse>(url, new Dictionary<string, string> { { "lang", "es" } });
        }

        private async Task<(Tracks.Video, Tracks.Audio, Tracks.Subtitle)> GetTracks(string id)
        {
            string mpdLink = string.Format(MpdUrl, id);

            string mpdFile = await HttpUtils.GetRequestStringResponse(mpdLink);

            XDocument doc = XDocument.Parse(mpdFile);

            var period = doc.Descendants(doc.Root.Name.Namespace + "Period").Where(v => v.Attribute("id").Value == "1").First();
            var res = period.Descendants(doc.Root.Name.Namespace + "AdaptationSet");

            var baseVideo = res.Where(x => x.Attribute("contentType")?.Value == "video");
            var baseAudio = res.Where(x => x.Attribute("contentType")?.Value == "audio");
            var baseSubtitles = res.Where(x => x.Attribute("mimeType")?.Value.StartsWith("text") == true);

            var pssh = GetPssh(baseVideo.Descendants(doc.Root.Name.Namespace + "ContentProtection"));
            var baseUrl = period.Descendants(doc.Root.Name.Namespace + "BaseURL").First().Value;

            return
            (
                Tracks.Video.Create(baseUrl, baseVideo.Descendants(doc.Root.Name.Namespace + "Representation"), pssh),
                Tracks.Audio.Create(baseUrl, doc, baseAudio),
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
            internal const int Multiple = 2;
        }
    }
}
