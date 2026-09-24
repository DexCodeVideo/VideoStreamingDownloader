using System.Collections.Generic;

namespace VideoStreamingDownloader._3CAT.DTOs.Response
{
    internal class MultipleMediaResponse
    {
        public Response Resposta { get; set; }
    }

    internal class Response
    {
        public Items Items { get; set; }
    }

    internal class Items
    {
        public List<Item> Item { get; set; }
    }

    internal class Item
    {
        public int Id { get; set; }
        public string Entradeta { get; set; }
        public string Titol { get; set; }
        public int Capitol { get; set; }
        public int Capitol_temporada { get; set; }
        public Temporades Temporades { get; set; }
        public string Durada { get; set; }
        public Programes Programes_tv { get; set; }
        public Programes Programes_radio { get; set; }
    }

    internal class Temporades : List<Temporada> { }

    internal class Temporada
    {
        public string Id { get; set; }
        public string Titol { get; set; }
    }

    internal class Programes : List<Programa> { }

    internal class Programa
    {
        public string Id { get; set; }
        public string Titol { get; set; }
    }
}
