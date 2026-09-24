using System;
using VideoStreamingDownloader.ETB.Media;

namespace VideoStreamingDownloader.ETB.Downloads
{
    internal class Item : Common.Downloads.Item
    {
        public Item(Track.Video video, Tracks.Audio audios, Tracks.Subtitle subtitles, FileInfo fileInfo)
        {
            Id = $"{fileInfo.Id}_ETB";
            Video = video;
            Audios = audios;
            Subtitles = subtitles;
            FileInfo = fileInfo;
            CreatedDate = DateTime.Now;
        }

        public Track.Video Video { get; private set; }
        public Tracks.Audio Audios { get; private set; }
        public Tracks.Subtitle Subtitles { get; private set; }
        public FileInfo FileInfo { get; private set; }
    }
}
