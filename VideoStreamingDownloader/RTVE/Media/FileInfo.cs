using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using VideoStreamingDownloader.RTVE.DTOs.Response;

namespace VideoStreamingDownloader.RTVE.Media
{
    internal class FileInfos : List<FileInfo>
    {
    }

    internal class FileInfo
    {
        [JsonInclude]
        public string Id { get; private set; }

        [JsonInclude]
        public string Title { get; private set; }

        [JsonInclude]
        public int Duration { get; private set; }

        [JsonConstructor]
        private FileInfo() { }

        internal FileInfo(string id, string title, int duration)
        {
            Id = id;
            Title = title;
            Duration = duration;
        }

        internal static FileInfo Create(MediaResponse mediaResponse)
        {
            var item = mediaResponse.Page.Items.FirstOrDefault();
            return new FileInfo(item.Id, item.LongTitle, item.Duration);
        }
    }
}
