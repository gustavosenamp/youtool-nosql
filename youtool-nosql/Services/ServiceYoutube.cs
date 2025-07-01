using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using youtool_nosql.Models;
using Comment = youtool_nosql.Models.Comment;

namespace youtool_nosql.Services
{
    public class ServiceYoutube
    {
        private readonly YouTubeService _youtubeApi;
        private readonly IMongoCollection<Comment> _commentCollection;
        private readonly IMongoCollection<ChannelInfo> _channelCollection;

        public ServiceYoutube(IConfiguration configuration, ServiceMongoDb mongoDbService)
        {
            _youtubeApi = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = configuration["YouTubeApiKey"],
                ApplicationName = "YouTubeNoSQL"
            });

            _commentCollection = mongoDbService.GetCollection<Comment>("Comments");
            _channelCollection = mongoDbService.GetCollection<ChannelInfo>("Channels");

        }

        public async Task<List<Comment>> ObterComentarios(string videoId)
        {
            var request = _youtubeApi.CommentThreads.List("snippet");
            request.VideoId = videoId;
            request.MaxResults = 100;
            request.TextFormat = CommentThreadsResource.ListRequest.TextFormatEnum.PlainText;

            var response = await request.ExecuteAsync();

            var comentarios = new List<Comment>();

            foreach (var item in response.Items)
            {
                var snippet = item.Snippet.TopLevelComment.Snippet;

                var comment = new Comment
                {
                    VideoId = videoId,
                    Author = snippet.AuthorDisplayName,
                    Text = snippet.TextDisplay,
                    PublishedAt = snippet.PublishedAt ?? DateTime.Now
                };

                comentarios.Add(comment);
                await _commentCollection.InsertOneAsync(comment);
            }

            return comentarios;
        }

        public async Task<ChannelInfo?> BuscarCanal(string termo)
        {
            var busca = _youtubeApi.Search.List("snippet");
            busca.Q = termo;
            busca.Type = "channel";
            busca.MaxResults = 1;

            var resultadoBusca = await busca.ExecuteAsync();
            var item = resultadoBusca.Items.FirstOrDefault();
            if (item == null) return null;

            var canalId = item.Snippet.ChannelId;
            var canalRequest = _youtubeApi.Channels.List("snippet,statistics,status,topicDetails,brandingSettings");
            canalRequest.Id = canalId;

            var resposta = await canalRequest.ExecuteAsync();
            var canal = resposta.Items.FirstOrDefault();
            if (canal == null) return null;

            var categorias = canal.TopicDetails?.TopicCategories?
                .Select(url => url.Split('/').Last().Replace("_", " "))?.ToList() ?? new();

            ChannelInfo model = new ChannelInfo
            {
                Nome = canal.Snippet.Title,
                Descricao = canal.Snippet.Description,
                UrlImagem = canal.Snippet.Thumbnails.Default__.Url,
                BannerUrl = canal.BrandingSettings?.Image?.BannerExternalUrl,
                DataCriacao = canal.Snippet.PublishedAtDateTimeOffset ?? DateTime.MinValue,
                Inscritos = canal.Statistics.SubscriberCount ?? 0,
                TotalVideos = canal.Statistics.VideoCount ?? 0,
                VisualizacoesTotais = canal.Statistics.ViewCount ?? 0,
                Pais = canal.Snippet.Country ?? "Desconhecido",
                Status = canal.Status?.PrivacyStatus ?? "Desconhecido",
                Categorias = categorias
            };

            await _channelCollection.InsertOneAsync(model);

            return model;
        }

        public async Task<List<Comment>> ListarComentarios()
        {
            return await _commentCollection.Find(_ => true).ToListAsync();
        }
    }
}
