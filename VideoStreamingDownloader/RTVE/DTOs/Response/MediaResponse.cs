using System.Collections.Generic;

namespace VideoStreamingDownloader.RTVE.DTOs.Response
{
    internal class MediaResponse
    {
        public Page Page { get; set; }
    }

    internal class Page
    {
        public List<Item> Items { get; set; }

        public int Number { get; set; }
        public int Size { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public int NumElements { get; set; }
    }

    internal class Item
    {
        public string Id { get; set; }
        public string LongTitle { get; set; }
        public int Duration { get; set; }
    }
}
