using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VideoStreamingDownloader.Common
{
    internal class Languages
    {
        private static Dictionary<string, Language> _languages = new Dictionary<string, Language>()
        {
            {"ca", new Language("cat") },
            {"eu", new Language("eus") },
            {"gl", new Language("glg") },
            {"en", new Language("eng") },
            {"fr", new Language("fra") },
            {"es", new Language("spa") }
        };

        public static Language Get(string code)
        {
            if (_languages.TryGetValue(code, out Language language))
                return language;

            return new Language(code);
        }
    }

    internal class Language
    {
        [JsonInclude]
        internal string IsoCode { get; private set; }
        [JsonIgnore]
        internal string Description => Resources.Translations.Strings.ResourceManager.GetString(IsoCode) ??
            string.Format(Resources.Translations.Strings.UndefinedLanguageCode, IsoCode);

        [JsonConstructor]
        private Language() { }

        internal Language(string isoCode)
        {
            IsoCode = isoCode;
        }
    }
}
