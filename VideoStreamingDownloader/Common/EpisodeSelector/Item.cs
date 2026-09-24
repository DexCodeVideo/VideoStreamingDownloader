using System.Collections.Generic;

namespace VideoStreamingDownloader.Common.EpisodeSelector
{
    internal class Items : List<Item> { }

    internal class Item
    {
        internal Item(string id, string title, int season, string seasonDescription = "")
        {
            Id = id;
            Title = title;
            Season = season;
            SeasonDescription = string.IsNullOrEmpty(seasonDescription) ? $"Temporada {season}" : seasonDescription;
        }

        public string Id { get; private set; }
        public string Title { get; private set; }
        public int Season { get; private set; }
        public string SeasonDescription { get; private set; }
    }
}
