using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nastaran_bot.Models;

public class Inspirations
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("telegramId")]
    public long TelegramId { get; set; }

    [BsonElement("label")]
    public string Label { get; set; }

    [BsonElement("imageFileId")]
    public string ImageFileId { get; set; }

    [BsonElement("caption")]
    public string Caption { get; set; }

    [BsonElement("tags")]
    public List<string> Tags { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("favorite")]
    public bool Favorite { get; set; }
}
