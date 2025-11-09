using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nastaran_bot.Models
{
    public class DailyNotes
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("date")]
        public string Date { get; set; }

        [BsonElement("text")]
        public string Text { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("author")]
        public string Author { get; set; }

        [BsonElement("used")]
        public bool Used { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}