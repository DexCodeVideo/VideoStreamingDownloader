using System.Collections.Generic;
using System.Text.Json.Serialization;
using VideoStreamingDownloader.ETB.DTOs.Response;

namespace VideoStreamingDownloader.ETB.Media
{
    internal class FileInfos : List<FileInfo>
    {
        internal static FileInfos Create(MultipleMediaResponse multipleMediaResponse)
        {
            FileInfos fileInfos = new FileInfos();

            foreach (var season in multipleMediaResponse.Seasons)
            {
                foreach (var episode in season.Episodes)
                {
                    fileInfos.Add(new FileInfo(episode.Slug, episode.Title, episode.Duration, season.Number, season.Title, multipleMediaResponse.Title));
                }
            }

            return fileInfos;
        }
    }

    internal class FileInfo
    {
        [JsonInclude]
        public string Id { get; private set; }
        [JsonInclude]
        public string Title { get; private set; }
        [JsonInclude]
        public int Duration { get; private set; }
        [JsonIgnore]
        public int Season { get; private set; }
        [JsonIgnore]
        public string SeasonDescription { get; private set; }
        [JsonIgnore]
        public string ProgramName { get; private set; }

        [JsonConstructor]
        private FileInfo() { }

        internal FileInfo(string id, string title, int duration, int season = 0, string seasonDescription = "", string programName = "")
        {
            Id = id;
            Title = string.IsNullOrEmpty(title) ? id : title;
            Duration = duration;
            Season = season;
            SeasonDescription = seasonDescription;
            ProgramName = programName;
        }

        internal static FileInfo Create(MediaResponse mediaResponse)
        {
            string title = string.IsNullOrEmpty(mediaResponse.Title) ? mediaResponse.Slug : mediaResponse.Title;
            return new FileInfo(mediaResponse.Id.ToString(), title, mediaResponse.Duration);
        }
    }
}
