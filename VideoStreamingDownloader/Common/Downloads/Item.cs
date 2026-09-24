using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VideoStreamingDownloader.Common.Downloads
{
    internal class Items : Dictionary<string, Item> { }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(_3CAT.Downloads.Item), "3CAT.DownloadItem")]
    [JsonDerivedType(typeof(RTVE.Downloads.Item), "RTVE.DownloadItem")]
    [JsonDerivedType(typeof(ETB.Downloads.Item), "ETB.DownloadItem")]
    internal class Item
    {
        public string Id { get; set; }
        public int State { get; set; } = States.Pending;
        public string DestinationPath { get; set; }
        public int RetryCount { get; set; } = 0;
        public string ErrorMessage { get; set; } = "";
        public DateTime CreatedDate { get; set; }
        public DateTime? StartedDate { get; set; } = null;
        public DateTime? EndDate { get; set; } = null;

        internal Item Clone()
        {
            return (Item)MemberwiseClone();
        }

        internal class States
        {
            public const int Pending = 0;
            public const int Downloading = 1;
            public const int Complete = 2;
            public const int Error = -1;

            private static readonly Dictionary<int, string> StateResources = new Dictionary<int, string>()
            {
                [Pending] = "Pending",
                [Downloading] = "Downloading",
                [Complete] = "Complete",
                [Error] = "Error"
            };

            internal static string Description(int state)
            {
                if (!StateResources.TryGetValue(state, out string resourceKey))
                    return Resources.Translations.Strings.Unknown;

                return Resources.Translations.Strings.ResourceManager.GetString(resourceKey) ?? Resources.Translations.Strings.Unknown;
            }
        }
    }
}
