using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace youtool_nosql.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string VideoId { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PublishedAt { get; set; }
    }
}
