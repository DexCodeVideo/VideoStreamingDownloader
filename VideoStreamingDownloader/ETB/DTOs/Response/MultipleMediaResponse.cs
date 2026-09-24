using System.Collections.Generic;

namespace VideoStreamingDownloader.ETB.DTOs.Response
{
    internal class MultipleMediaResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public Seasons Seasons { get; set; }
    }

    internal class Seasons : List<Season> { }

    internal class Season
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public Episodes Episodes { get; set; }
    }
    internal class Episodes : List<Episode> { }

    internal class Episode
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Duration { get; set; }
    }
}
