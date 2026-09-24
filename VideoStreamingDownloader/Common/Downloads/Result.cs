using System.Collections.Generic;
using System.Linq;

namespace VideoStreamingDownloader.Common.Downloads
{
    internal class Results : List<Result<IMetadata>>
    {
        internal List<Result<G>> GetTypeResults<G>() where G : IMetadata
        {
            return this
            .Where(r => r.Metadata is G)
            .Select(r => new Result<G>((G)r.Metadata, r.Path))
            .ToList();
        }
    }

    internal class Result<T>
    {
        internal string Path { get; private set; }
        internal T Metadata { get; private set; }

        internal Result(T metadata, string path)
        {
            this.Metadata = metadata;
            this.Path = path;
        }
    }

    internal interface IMetadata { }

    internal class VideoMetada : IMetadata { }

    internal class AudioMetada : IMetadata
    {
        internal AudioMetada(string languageIsoCode)
        {
            LanguageIsoCode = languageIsoCode;
        }

        internal string LanguageIsoCode { get; private set; }
    }

    internal class SubtitleMetada : IMetadata
    {
        internal SubtitleMetada(string languageIsoCode)
        {
            LanguageIsoCode = languageIsoCode;
        }

        internal string LanguageIsoCode { get; private set; }
    }
}
