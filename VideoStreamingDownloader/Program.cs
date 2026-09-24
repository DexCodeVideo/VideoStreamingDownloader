using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using VideoStreamingDownloader.Common.Downloads;
using VideoStreamingDownloader.Common.Logger;

namespace VideoStreamingDownloader
{
    internal static class Program
    {
        internal static string ResourcesDirectory = Path.Combine(AppContext.BaseDirectory, "Resources");
        internal static string UserDataDirectory = Path.Combine(AppContext.BaseDirectory, "UserData");

        internal static UserSettings UserSettings { get; private set; }
        internal static Manager DownloadManager { get; private set; }
        internal static LoggerService LoggerService { get; private set; }

        internal static void SetLanguage(string language)
        {
            CultureInfo.CurrentUICulture = new CultureInfo(language);
            CultureInfo.CurrentCulture = new CultureInfo(language);
        }

        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LoggerService = new LoggerService();
            LoggerService.LogInformation("Loading app");

            Directory.CreateDirectory(UserDataDirectory);
            UserSettings = UserSettings.Get();
            DownloadManager = Manager.Get();
            SetLanguage(UserSettings.Language);

            LoggerService.LogInformation("App started");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                Application.Run(new Main());
                LoggerService.LogInformation("App closed");
            }
            catch (Exception ex)
            {
                LoggerService.LogError("App closed by exception", ex);
                throw;
            }
        }
    }
}
