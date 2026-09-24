using System.Collections.Generic;

namespace VideoStreamingDownloader.ETB.DTOs.Response
{
    internal class MediaResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Duration { get; set; }
        public Manifests Manifests { get; set; }
    }

    internal class Manifests : List<Manifest> { }

    internal class Manifest
    {
        public DrmConfig DrmConfig { get; set; }
    }

    internal class DrmConfig
    {
        public string Type { get; set; }
        public string licenseAcquisitionURL { get; set; }
    }
}
