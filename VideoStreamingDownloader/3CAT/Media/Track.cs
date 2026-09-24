using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using VideoStreamingDownloader._3CAT.DTOs.Response;
using VideoStreamingDownloader.Common;
using VideoStreamingDownloader.Common.Downloads;

namespace VideoStreamingDownloader._3CAT.Media
{
    internal class Track
    {
        internal class Video
        {
            private const string NoVideo = "No video";

            [JsonConstructor]
            private Video() { }

            internal Video(string resolution, Urls urls)
            {
                Resolution = resolution;
                Urls = urls;
            }

            internal Video(string resolution, string directLink)
            {
                Resolution = resolution;
                DirectLink = directLink;
            }

            internal static Video CreateNoVideo()
            {
                return new Video(NoVideo, string.Empty);
            }

            [JsonInclude]
            public string Resolution { get; private set; }

            [JsonIgnore]
            public Urls Urls { get; private set; }

            [JsonIgnore]
            public string DirectLink { get; private set; } = string.Empty;

            [JsonIgnore]
            public bool HasVideo => Resolution != NoVideo;
        }

        internal class Audio
        {
            [JsonConstructor]
            private Audio() { }

            internal Audio(Language language, Urls urls)
            {
                Language = language;
                Urls = urls;
            }

            internal Audio(Language language, string directLink)
            {
                Language = language;
                DirectLink = directLink;
            }

            [JsonInclude]
            public Language Language { get; private set; }

            [JsonIgnore]
            public Urls Urls { get; private set; }

            [JsonIgnore]
            public string DirectLink { get; private set; } = string.Empty;
        }

        internal class Subtitle
        {
            [JsonConstructor]
            private Subtitle() { }

            internal Subtitle(Language language, string format, string directLink)
            {
                Language = language;
                Format = format;
                DirectLink = directLink;
            }

            [JsonInclude]
            public Language Language { get; private set; }

            [JsonIgnore]
            public string Format { get; private set; }
            [JsonIgnore]
            public string DirectLink { get; private set; } = string.Empty;
        }
    }

    internal class Tracks
    {
        internal class Video : List<Track.Video>
        {
            internal static Video Create(SingleMediaResponse mediaInfoResponse)
            {
                var videos = new Video();
                videos.Add(Track.Video.CreateNoVideo());
                foreach (Url videoUrl in mediaInfoResponse.Media.Url.Where(x => x.File.EndsWith("mp4")))
                {
                    videos.Add(new Track.Video(videoUrl.Label, videoUrl.File));
                }

                return videos;
            }

            internal static Video Create(IEnumerable<XElement> doc, string domainUrl)
            {
                Video videos = new Video();
                videos.Add(Track.Video.CreateNoVideo());
                foreach (XElement element in doc)
                {
                    videos.Add(new Track.Video(
                        element.Attribute("height").Value + "p",
                        BuildDownloadUrls(element.Descendants().First(), domainUrl)
                        )
                    );
                }

                return videos;
            }
        }

        internal class Audio : List<Track.Audio>
        {
            public Audio() : base() { }
            internal Audio(IEnumerable<Track.Audio> audios) : base(audios) { }
            internal static Audio Create(SingleMediaResponse mediaInfoResponse)
            {
                var audios = new Audio();

                foreach (Url videoUrl in mediaInfoResponse.Media.Url.Where(x => x.File.EndsWith("mp3")))
                {
                    audios.Add(new Track.Audio(Languages.Get("ca"), videoUrl.File));
                }

                if (audios.Count == 0)
                    audios.Add(new Track.Audio(Languages.Get("ca"), string.Empty));

                return audios;
            }
            internal static Audio Create(IEnumerable<XElement> doc, string domainUrl)
            {
                Audio audios = new Audio();
                foreach (XElement element in doc)
                {
                    audios.Add(new Track.Audio(
                        Languages.Get(element.Attribute("lang").Value),
                        BuildDownloadUrls(element.Descendants().Last(), domainUrl)
                        )
                    );
                }

                return audios;
            }
        }

        internal class Subtitle : List<Track.Subtitle>
        {
            public Subtitle() : base() { }
            internal Subtitle(IEnumerable<Track.Subtitle> subtitles) : base(subtitles) { }

            internal static Subtitle Create(SingleMediaResponse mediaInfoResponse)
            {
                var subtitleList = new Subtitle();
                foreach (Subtitol sub in mediaInfoResponse.Subtitols)
                {
                    subtitleList.Add(new Track.Subtitle(Languages.Get(sub.Iso), sub.Format, sub.Url));
                }
                return subtitleList;
            }
        }

        private static Urls BuildDownloadUrls(XElement element, string domainUrl)
        {
            return new Urls(domainUrl + element.Attribute("initialization").Value,
                                    domainUrl + element.Attribute("media").Value.Replace("$Number$", "{0}"),
                                    int.Parse(element.Attribute("startNumber").Value));
        }
    }
}
