using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nastaran_bot.Models;

public class Inspiration
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id
    {
        get;
        set;
    } = string.Empty;

    [BsonElement("telegramId")]
    public long TelegramId
    {
        get;
        set;
    }

    [BsonElement("label")]
    public string Label
    {
        get;
        set;
    } = string.Empty;

    [BsonElement("imageFileId")]
    public string ImageFileId
    {
        get;
        set;
    } = string.Empty;

    [BsonElement("content")]
    public string Content
    {
        get;
        set;
    } = string.Empty;

    [BsonElement("tags")]
    public List<string> Tags
    {
        get;
        set;
    } = [];

    [BsonElement("createdAt")]
    public DateTime CreatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;

    [BsonElement("favorite")]
    public bool Favorite
    {
        get;
        set;
    } = false;
}
