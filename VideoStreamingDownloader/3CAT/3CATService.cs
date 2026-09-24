using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using VideoStreamingDownloader._3CAT.DTOs.Response;
using VideoStreamingDownloader._3CAT.Media;
using VideoStreamingDownloader.Common;

namespace VideoStreamingDownloader._3CAT
{
    internal class _3CATService
    {
        private const string BaseVideoInfoUrl = "https://api-media.3cat.cat/pvideo/media.jsp?media=video&versio=vast&idint={0}&profile=pc_3cat&format=dm";
        private const string EpisodeListUrl = "https://www.3cat.cat/api/3cat/dades/";
        private const string QueryEpisodeListFormat = "[\"tira\",{\"url\":\"https://api.3cat.cat/videos?_format=json&ordre=capitol&origen=llistat&tipus_contingut=PPD&version=2.0&programatv_id={0}&items_pagina=2000&pagina=1\"}]";
        private const string BaseAudioInfoUrl = "https://api-media.3cat.cat/pvideo/media.jsp?media=audio&versio=vast&idint={0}&profile=pc_3cat&format=dm";
        private const string QueryAudioListFormat = "[\"tira\",{\"url\":\"https://api.3cat.cat/audios?_format=json&ordre=-data_publicacio&origen=llistat&programaradio_id={0}&tipus_audio=CRTAPROG&items_pagina=10000&pagina=1&sdom=img&version=2.0&cache=180&https=true&master=yes\"}]";

        internal int _id;
        private int _mediaType = MediaTypeCodes.None;

        internal _3CATService() { }

        internal _3CATService(int id)
        {
            _id = id;
            _mediaType = MediaTypeCodes.Single;
        }

        internal async Task<int> IdentifyUrl(string url)
        {
            int score = 0;

            if (url.ToLower().Contains("3cat"))
                score++;

            try
            {
                _id = GetIdFromUrl(url);
                await ValidateEpisode(_id);
                _mediaType = MediaTypeCodes.Single;
                score++;
            }
            catch { }
            try
            {
                _id = await GetProgramId(url);
                _mediaType = MediaTypeCodes.Multiple;
                score++;
            }
            catch { }

            return score;
        }

        private int GetIdFromUrl(string url)
        {
            string[] splitedUrl = url.Split('/');
            int code;

            if (!int.TryParse(splitedUrl[splitedUrl.Length - 1], out code))
                if (!int.TryParse(splitedUrl[splitedUrl.Length - 2], out code))
                    throw new Exception("Episode not found");

            return code;
        }

        private async Task ValidateEpisode(int episodeCode)
        {
            using (HttpClient client = new HttpClient())
            {
                var videoTask = client.GetAsync(string.Format(BaseVideoInfoUrl, episodeCode));
                var audioTask = client.GetAsync(string.Format(BaseAudioInfoUrl, episodeCode));

                await Task.WhenAll(videoTask, audioTask);

                var videoResponse = await videoTask;
                var audioResponse = await audioTask;

                if (!videoResponse.IsSuccessStatusCode && !audioResponse.IsSuccessStatusCode)
                    throw new Exception("Episode not found");
            }
        }

        private async Task<int> GetProgramId(string url)
        {
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync(url);

                if (!res.IsSuccessStatusCode)
                    throw new Exception("ProgramId page error");

                var content = await res.Content.ReadAsStringAsync();
                var match = Regex.Match(content, "\"programaId\":([0-9]*)");

                if (!match.Success)
                    throw new Exception("ProgramId not found");

                return int.Parse(match.Groups[1].Value);
            }
        }

        private async Task<SingleMediaResponse> GetSingleMediaResponse(int episodeCode)
        {
            string videoUrl = string.Format(BaseVideoInfoUrl, episodeCode);
            string audioUrl = string.Format(BaseAudioInfoUrl, episodeCode);

            var videoTask = HttpUtils.GetRequestDeserializedResponse<SingleMediaResponse>(videoUrl);
            var audioTask = HttpUtils.GetRequestDeserializedResponse<SingleAudioMediaResponse>(audioUrl);

            await Task.WhenAll(videoTask, audioTask);

            var videoResponse = await videoTask;
            var audioResponse = await audioTask;

            if (videoResponse.Informacio is null)
            {
                return new SingleMediaResponse()
                {
                    Informacio = audioResponse.Informacio,
                    Subtitols = audioResponse.Subtitols,
                    Media = new DTOs.Response.Media() { Url = new List<Url>() { new Url { File = audioResponse.Media.Url, Label = "MP3" } } }
                };
            }
            else
            {
                return videoResponse;
            }
        }

        internal async Task<Media.Options> GetMediaOptions()
        {
            int episodeCode = _id;
            MultipleMediaResponse multipleMediaResponse = null;
            FileInfos fileInfos = new FileInfos();

            if (_mediaType == MediaTypeCodes.Multiple)
            {
                multipleMediaResponse = await GetAllEpisodesFromProgram(_id);
                episodeCode = multipleMediaResponse.Resposta.Items.Item.Last().Id;
                fileInfos = FileInfos.Create(multipleMediaResponse);
            }

            SingleMediaResponse singleMediaResponse = await GetSingleMediaResponse(episodeCode);

            if (fileInfos.Count == 0)
                fileInfos.Add(FileInfo.Create(singleMediaResponse));

            var (videos, audios, subtitles) = await GetTracks(singleMediaResponse);

            return new Media.Options()
            {
                Videos = videos,
                Audios = audios,
                Subtitles = subtitles,
                FileInfos = fileInfos,
            };
        }

        private async Task<(Tracks.Video, Tracks.Audio, Tracks.Subtitle)> GetTracks(SingleMediaResponse singleMediaResponse)
        {
            Tracks.Video videos;
            Tracks.Audio audios;
            string mpdLink = ObtainMpdUrl(singleMediaResponse);

            if (!string.IsNullOrEmpty(mpdLink))
            {
                string mpdFile = await HttpUtils.GetRequestStringResponse(mpdLink);
                string domainUrl = mpdLink.Replace("stream.mpd", "");

                XDocument doc = XDocument.Parse(mpdFile);

                var res = doc.Descendants(doc.Root.Name.Namespace + "AdaptationSet");

                var baseVideo = res.Where(x => x.Attribute("mimeType").Value == "video/mp4");
                var baseAudio = res.Where(x => x.Attribute("mimeType").Value == "audio/mp4");


                videos = Tracks.Video.Create(baseVideo.Descendants(doc.Root.Name.Namespace + "Representation"), domainUrl);
                audios = Tracks.Audio.Create(baseAudio, domainUrl);
            }
            else
            {
                videos = Tracks.Video.Create(singleMediaResponse);
                audios = Tracks.Audio.Create(singleMediaResponse);
            }

            var subs = singleMediaResponse.Subtitols is null ? new Tracks.Subtitle() : Tracks.Subtitle.Create(singleMediaResponse);

            return (videos, audios, subs);
        }

        private async Task<MultipleMediaResponse> GetAllEpisodesFromProgram(int programId)
        {
            UriBuilder episodesUriBuilder = BuildProgramQuery(QueryEpisodeListFormat, programId);
            UriBuilder audiosUriBuilder = BuildProgramQuery(QueryAudioListFormat, programId);

            var episodesTask = HttpUtils.GetRequestDeserializedResponse<MultipleMediaResponse>(episodesUriBuilder.ToString());
            var audiosTask = HttpUtils.GetRequestDeserializedResponse<MultipleMediaResponse>(audiosUriBuilder.ToString());

            await Task.WhenAll(episodesTask, audiosTask);

            var episodesResponse = await episodesTask;
            var audiosResponse = await audiosTask;

            if (episodesResponse.Resposta.Items.Item.Count > 0)
                return episodesResponse;
            else if (audiosResponse.Resposta.Items.Item.Count > 0)
                return audiosResponse;

            throw new Exception($"No media found for program: {programId}");
        }

        private UriBuilder BuildProgramQuery(string format, int programId)
        {
            UriBuilder uriBuilder = new UriBuilder(EpisodeListUrl);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["queryKey"] = format.Replace("{0}", programId.ToString());
            uriBuilder.Query = query.ToString();

            return uriBuilder;
        }
        private string ObtainMpdUrl(SingleMediaResponse mediaInfoResponse)
        {
            Url url = mediaInfoResponse.Media.Url.Where(x => x.File.EndsWith(".mpd")).FirstOrDefault();
            return url == null ? string.Empty : url.File;
        }

        private class MediaTypeCodes
        {
            internal const int None = 0;
            internal const int Single = 1;
            internal const int Multiple = 2;
        }
    }
}
