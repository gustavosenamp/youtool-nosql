using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace youtool_nosql.Models
{
    public class ChannelInfo
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public DateTimeOffset DataCriacao { get; set; }
        public string UrlImagem { get; set; }
        public string BannerUrl { get; set; }
        public ulong Inscritos { get; set; }
        public ulong TotalVideos { get; set; }
        public ulong VisualizacoesTotais { get; set; }
        public string Pais { get; set; }
        public string Status { get; set; }
        public List<string> Categorias { get; set; } = new();
    }
}
