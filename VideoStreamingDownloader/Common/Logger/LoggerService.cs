using System;
using System.IO;
using System.Text;

namespace VideoStreamingDownloader.Common.Logger
{
    internal class LoggerService
    {
        private readonly string _logDirectory;
        private readonly object _lock = new object();

        internal LoggerService()
        {
            _logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");
            Directory.CreateDirectory(_logDirectory);
        }

        internal void LogInformation(string message)
        {
            Log(Type.Codes.Information, message);
        }

        internal void LogWarning(string message)
        {
            Log(Type.Codes.Warning, message);
        }

        internal void LogError(string message, Exception exception = null)
        {
            Log(Type.Codes.Error, $"{message}{Environment.NewLine}Exception: {exception?.ToString() ?? "Null exception"}");
        }

        internal void LogError(Exception exception)
        {
            Log(Type.Codes.Error, exception?.ToString() ?? "Null exception");
        }


        private void Log(int type, string message)
        {
            lock (_lock)
            {
                string filePath = GetTodayLogFilePath();

                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string line = $"[{timestamp}][{Type.Descriptions(type)}] {message}{Environment.NewLine}";

                File.AppendAllText(
                    filePath,
                    line,
                    Encoding.UTF8);
            }
        }

        private string GetTodayLogFilePath()
        {
            string fileName = $"{DateTime.Now:yyyy-MM-dd}.txt";
            return Path.Combine(_logDirectory, fileName);
        }

        internal class Type
        {
            internal class Codes
            {
                internal const int Information = 0;
                internal const int Warning = 1;
                internal const int Error = 2;
            }

            internal static string Descriptions(int type)
            {
                switch (type)
                {
                    case Codes.Information:
                        return "Information";
                    case Codes.Warning:
                        return "Warning";
                    case Codes.Error:
                        return "Error";
                    default:
                        return "Unknown";
                }
            }
        }
    }
}
