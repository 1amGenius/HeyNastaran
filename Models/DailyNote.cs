using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nastaran_bot.Models;

public class DailyNote
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
        get; set;
    }

    [BsonElement("date")]
    public string Date 
    { 
        get;
        set;
    } = string.Empty;

    [BsonElement("text")]
    public string Text 
    {
        get;
        set; 
    } = string.Empty;

    [BsonElement("category")]
    public string Category 
    {
        get;
        set;
    } = string.Empty;

    [BsonElement("author")]
    public string Author 
    {
        get; 
        set;
    } = "unknown";

    [BsonElement("usedAt")]
    [BsonIgnoreIfNull]
    public DateTime? UsedAt
    {
        get; set;
    }

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
}
