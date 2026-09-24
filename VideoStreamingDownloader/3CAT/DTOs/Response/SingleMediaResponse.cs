using System.Collections.Generic;

namespace VideoStreamingDownloader._3CAT.DTOs.Response
{
    internal class SingleMediaResponse
    {
        public Informacio Informacio { get; set; }
        public Subtitols Subtitols { get; set; }
        public Media Media { get; set; }
    }

    internal class SingleAudioMediaResponse
    {
        public Informacio Informacio { get; set; }
        public Subtitols Subtitols { get; set; }
        public AudioMedia Media { get; set; }
    }

    internal class Informacio
    {
        public string Titol { get; set; }
        public string Titol_complet { get; set; }
        public int Id { get; set; }
        public Durada Durada { get; set; }
    }

    internal class Durada
    {
        public int Milisegons { get; set; }
    }

    internal class Subtitols : List<Subtitol>
    {

    }

    internal class Subtitol
    {
        public string Iso { get; set; }
        public string Url { get; set; }
        public string Format { get; set; }
    }

    internal class Media
    {
        public List<Url> Url { get; set; }
    }

    internal class AudioMedia
    {
        public string Url { get; set; }
    }

    internal class Url
    {
        public string File { get; set; }
        public string Label { get; set; }
    }
}
