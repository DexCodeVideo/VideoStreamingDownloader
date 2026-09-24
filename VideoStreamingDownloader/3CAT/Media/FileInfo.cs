using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using VideoStreamingDownloader._3CAT.DTOs.Response;

namespace VideoStreamingDownloader._3CAT.Media
{
    internal class FileInfos : List<FileInfo>
    {
        internal static FileInfos Create(MultipleMediaResponse multipleMediaResponse)
        {
            FileInfos fileInfos = new FileInfos();

            foreach (var item in multipleMediaResponse.Resposta.Items.Item.OrderBy(x => x.Capitol))
            {
                fileInfos.Add(new FileInfo(item.Id.ToString(), item.Titol, GetDuration(item.Durada), GetSeason(item), item.Programes_tv?.FirstOrDefault()?.Titol ?? item.Programes_radio?.FirstOrDefault()?.Titol));
            }

            return fileInfos;
        }

        private static int GetSeason(Item item)
        {
            if (TryFindSeason(item.Entradeta, out int temporada))
                return temporada;

            var temporades = item.Temporades;
            if (temporades is null || temporades.Count == 0)
                return 0;
            try
            {
                return int.Parse(temporades.First().Id.Split('_')[1]);
            }
            catch (FormatException) { return 0; }
        }

        private static bool TryFindSeason(string description, out int season)
        {
            season = 0;

            if (string.IsNullOrEmpty(description))
                return false;

            Match match = Regex.Match(description, @"(\d+)x(\d+)");

            if (match.Success)
            {
                season = int.Parse(match.Groups[1].Value);
                //int episode = int.Parse(match.Groups[2].Value);                
            }

            return match.Success;
        }

        private static int GetDuration(string duration)
        {
            string format = @"hh\:mm\:ss\:ff";
            if (TimeSpan.TryParseExact(duration, format, CultureInfo.InvariantCulture, out TimeSpan result))
                return (int)result.TotalMilliseconds;
            return 0;
        }
    }

    internal class FileInfo
    {
        [JsonInclude]
        public string Id { get; private set; }
        [JsonInclude]
        public string Title { get; private set; }
        [JsonInclude]
        public int Duration { get; private set; }
        [JsonInclude]
        public int Season { get; private set; }
        [JsonInclude]
        public string ProgramName { get; private set; }

        [JsonConstructor]
        private FileInfo() { }

        internal FileInfo(string id, string title, int duration, int season = 0, string programName = "")
        {
            Id = id;
            Title = title;
            Duration = duration;
            Season = season;
            ProgramName = programName;
        }

        internal static FileInfo Create(SingleMediaResponse mediaInfoResponse)
        {
            return new FileInfo(mediaInfoResponse.Informacio.Id.ToString(), mediaInfoResponse.Informacio.Titol, mediaInfoResponse.Informacio.Durada.Milisegons);
        }
    }
}
