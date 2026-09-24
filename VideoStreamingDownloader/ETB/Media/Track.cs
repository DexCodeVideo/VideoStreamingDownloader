using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Xml.Linq;
using VideoStreamingDownloader.Common;
using VideoStreamingDownloader.Common.Downloads;

namespace VideoStreamingDownloader.ETB.Media
{
    internal class Track
    {
        internal class Video
        {
            private const string NoVideo = "No video";

            [JsonConstructor]
            private Video() { }

            public Video(string resolution, Urls urls, string pssh)
            {
                Resolution = resolution;
                Urls = urls;
                Pssh = pssh;
            }

            internal static Video CreateNoVideo(string pssh)
            {
                return new Video(NoVideo, null, pssh);
            }

            [JsonInclude]
            public string Resolution { get; private set; }

            [JsonIgnore]
            public string Pssh { get; private set; }

            [JsonIgnore]
            public Urls Urls { get; private set; }

            [JsonIgnore]
            public bool HasVideo => Resolution != NoVideo;
        }

        internal class Audio
        {
            [JsonConstructor]
            private Audio() { }

            public Audio(Language language, Urls urls)
            {
                Language = language;
                Urls = urls;
            }


            [JsonInclude]
            public Language Language { get; private set; }

            [JsonIgnore]
            public Urls Urls { get; private set; }
        }

        internal class Subtitle
        {
            [JsonConstructor]
            private Subtitle() { }

            public Subtitle(Language language, string directLink)
            {
                Language = language;
                DirectLink = directLink;
            }

            [JsonInclude]
            public Language Language { get; private set; }

            [JsonIgnore]
            public string DirectLink { get; private set; }
        }
    }

    internal class Tracks
    {
        internal class Video : List<Track.Video>
        {
            internal static Video Create(string baseUrl, IEnumerable<XElement> doc, string pssh)
            {
                Video videos = new Video();
                videos.Add(Track.Video.CreateNoVideo(pssh));
                foreach (XElement element in doc)
                {
                    videos.Add(new Track.Video(
                        element.Attribute("height").Value + "p",
                        BuildDownloadUrls(baseUrl, element.Descendants().First()),
                        pssh)
                    );
                }

                return videos;
            }
        }

        internal class Audio : List<Track.Audio>
        {
            public Audio() : base() { }
            internal Audio(IEnumerable<Track.Audio> audios) : base(audios) { }

            internal static Audio Create(string baseUrl, XDocument doc, IEnumerable<XElement> docs)
            {
                Audio audios = new Audio();
                foreach (XElement element in docs)
                {
                    audios.Add(new Track.Audio(
                        Languages.Get(element.Attribute("lang").Value),
                        BuildDownloadUrls(baseUrl, element.Descendants(doc.Root.Name.Namespace + "SegmentTemplate").First())
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

            internal static Subtitle Create(XDocument doc, IEnumerable<XElement> docs)
            {
                var subtitleList = new Subtitle();
                foreach (XElement element in docs)
                {
                    subtitleList.Add(new Track.Subtitle(
                        Languages.Get(element.Attribute("lang").Value),
                        element.Descendants(doc.Root.Name.Namespace + "BaseURL").First().Value
                        ));
                }
                return subtitleList;
            }
        }

        internal static Urls BuildDownloadUrls(string baseUrl, XElement element)
        {
            return new Urls(baseUrl + element.Attribute("initialization").Value,
                                    baseUrl + element.Attribute("media").Value.Replace("$Number$", "{0}"),
                                    int.Parse(element.Attribute("startNumber").Value));
        }
    }
}
