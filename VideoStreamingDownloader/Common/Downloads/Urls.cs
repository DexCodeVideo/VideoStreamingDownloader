namespace VideoStreamingDownloader.Common.Downloads
{
    internal class Urls
    {
        public string InitUrl { get; private set; }
        public string DataUrl { get; private set; }
        public int SegmentStart { get; private set; }

        public Urls(string initUrl, string dataUrl, int segmentStart)
        {
            InitUrl = initUrl;
            DataUrl = dataUrl;
            SegmentStart = segmentStart;
        }
    }
}
