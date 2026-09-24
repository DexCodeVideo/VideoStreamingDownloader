using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace VideoStreamingDownloader
{
    internal class UserSettings
    {
        private static readonly string _settingsFile = Path.Combine(Program.UserDataDirectory, "Settings.json");

        internal string TempPath { get; private set; } = "C:\\Temp";
        internal string DefaultDownloadPath { get; private set; } = GetDownloadPath();
        internal int SimultaneousDownloads { get; private set; } = 5;
        internal string Language { get; private set; } = "ca";

        internal class UserSettingsDto
        {
            public string TempPath { get; set; }
            public string DefaultDownloadPath { get; set; }
            public int SimultaneousDownloads { get; set; }
            public string Language { get; set; }
        }

        internal static UserSettings Get()
        {
            if (!File.Exists(_settingsFile))
            {
                Program.LoggerService.LogWarning("No UserSettings file found");
                return new UserSettings();
            }

            try
            {
                string data = File.ReadAllText(_settingsFile);
                return Get(JsonSerializer.Deserialize<UserSettingsDto>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }));
            }
            catch (Exception ex)
            {
                {
                    Program.LoggerService.LogError("UserSettings failed", ex);
                    return new UserSettings();
                }
            }
        }

        internal static UserSettings Get(UserSettingsDto userSettingsDto)
        {
            return new UserSettings
            {
                DefaultDownloadPath = userSettingsDto.DefaultDownloadPath,
                SimultaneousDownloads = userSettingsDto.SimultaneousDownloads,
                TempPath = userSettingsDto.TempPath,
                Language = userSettingsDto.Language,
            };
        }

        internal void Save(UserSettingsDto userSettingsDto)
        {
            TempPath = userSettingsDto.TempPath;
            DefaultDownloadPath = userSettingsDto.DefaultDownloadPath;
            SimultaneousDownloads = userSettingsDto.SimultaneousDownloads;
            Language = userSettingsDto.Language;

            File.WriteAllText(_settingsFile, JsonSerializer.Serialize(userSettingsDto));
        }

        private static string GetDownloadPath()
        {
            return SHGetKnownFolderPath(new Guid("374DE290-123F-4565-9164-39C4925E467B"), 0);
        }

        [DllImport("shell32", CharSet = CharSet.Unicode,
                    ExactSpelling = true, PreserveSig = false)]
        private static extern string SHGetKnownFolderPath(
            [MarshalAs(UnmanagedType.LPStruct)]
         Guid rfid, uint dwFlags, uint hToken = default);
    }
}
