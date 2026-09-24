using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VideoStreamingDownloader.Common.Downloads
{
    internal class Utils
    {
        private readonly static string _decryptFile = Path.Combine(Program.ResourcesDirectory, "mp4decrypt.exe");
        private readonly static string _ffmpegFile = Path.Combine(Program.ResourcesDirectory, "ffmpeg.exe");
        private readonly static string _l3KeyExtractFile = Path.Combine(Program.ResourcesDirectory, "l3-cli.exe");
        private readonly static string _CDMsFolder = Path.Combine(Program.ResourcesDirectory, "CDMs");

        internal static string GetTempFolder(string originId, string fileId)
        {
            return $"{VideoStreamingDownloader.Program.UserSettings.TempPath}\\VideoStreamingDownloader\\{originId}\\{fileId}";
        }

        internal static string CreateFilename(string destinationPath, string title)
        {
            string _title = string.IsNullOrEmpty(title) ? Guid.NewGuid().ToString() : title;
            return $"{destinationPath}\\{GetSafeFilename(_title)}.mp4";
        }

        private static string GetSafeFilename(string filename)
        {
            return string.Join("", filename.Split(Path.GetInvalidFileNameChars()));
        }

        internal static void CreateDirectory(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        internal static void DecryptFile(string filepath, string decryptKey)
        {
            string tempFile = Path.GetDirectoryName(filepath) + $"\\{Guid.NewGuid()}.mp4";
            var command = $"--key {decryptKey} \"{filepath}\" \"{tempFile}\"";

            RunProcess(_decryptFile, command);

            File.Delete(filepath);
            File.Move(tempFile, filepath);
        }

        internal static string ExtractVtt(string source)
        {
            string destination = Path.GetDirectoryName(source) + $"\\{Guid.NewGuid()}.vtt";
            FragmentedWebVttExtractor.Extract(source, destination);
            File.Delete(source);

            return destination;
        }

        internal static void MergeFile(Results results, string destinationPath)
        {
            string command = BuildFfmpegCommand(results, destinationPath);

            try
            {
                RunProcess(_ffmpegFile, command);
            }
            catch (Exception ex)
            {
                File.Delete(destinationPath);
                throw ex;
            }
        }

        private static string BuildFfmpegCommand(Results results, string destinationPath)
        {
            Result<VideoMetada> video = results.GetTypeResults<VideoMetada>().FirstOrDefault();
            List<Result<AudioMetada>> audios = results.GetTypeResults<AudioMetada>();
            List<Result<SubtitleMetada>> Subtitles = results.GetTypeResults<SubtitleMetada>();

            StringBuilder sb = new StringBuilder($"-hide_banner -loglevel error -y ");

            int mapCount = 0;
            bool HasVideo = video != null;

            if (HasVideo)
            {
                sb.Append($"-i \"{video.Path}\" ");
                mapCount++;
            }

            foreach (var audio in audios)
            {
                if (!string.IsNullOrEmpty(audio.Path))
                {
                    sb.Append($"-i \"{audio.Path}\" ");
                    mapCount++;
                }
            }

            foreach (var sub in Subtitles)
            {
                sb.Append($"-i \"{sub.Path}\" ");
                mapCount++;
            }

            sb.Append("-c:v copy -c:a copy ");

            if (Subtitles.Count > 0)
                sb.Append("-c:s mov_text ");

            for (int i = 0; i < mapCount; i++)
            {
                sb.Append($"-map {i} ");
            }

            int audioTrack = 0;
            foreach (var audio in audios)
            {
                sb.Append($"-metadata:s:a:{audioTrack} language={audio.Metadata.LanguageIsoCode} ");
                audioTrack++;
            }

            int subTrack = 0;
            foreach (var sub in Subtitles)
            {
                sb.Append($"-metadata:s:s:{subTrack} language={sub.Metadata.LanguageIsoCode} ");
                subTrack++;
            }

            sb.Append($"\"{destinationPath}\"");

            return sb.ToString();
        }

        internal static string ObtainDecryptiontKey(string url, string pssh)
        {
            string wvdFile = Directory.GetFiles(_CDMsFolder).FirstOrDefault();

            if (wvdFile == null)
                throw new Exception("Missing wvd file!");

            var command = $"-wvd \"{wvdFile}\" -pssh {pssh} -lic_url {url}";
            string output = RunProcess(_l3KeyExtractFile, command);

            var match = Regex.Match(output, "\"([^\"]*)\"");
            if (!match.Success || match.Groups.Count < 2)
                throw new Exception("Key match failed");
            return match.Groups[1].Value;
        }

        private static string RunProcess(string file, string command)
        {
            string output = "";

            using (Process p = new Process())
            {
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.FileName = file;
                p.StartInfo.Arguments = command;

                p.Start();
                p.WaitForExit();

                output = p.StandardOutput.ReadToEnd();

                if (p.ExitCode != 0)
                {
                    string error = p.StandardError.ReadToEnd();
                    throw new Exception(
                        $"Process {file} failed with exit code {p.ExitCode}.\n\n" +
                        $"Command: {command}\n\n" +
                        $"Error:\n{error}");
                }
            }

            return output;
        }
    }
}
