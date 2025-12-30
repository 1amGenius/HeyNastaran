using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Contracts.Models;

/// <summary>
/// Represents a stored idea with metadata, content, and tagging support.
/// </summary>
public sealed class Idea
{
    /// <summary>
    /// Unique identifier for the idea document.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Telegram user identifier associated with the idea.
    /// </summary>
    [BsonElement("telegramId")]
    public long TelegramId
    {
        get;
        set;
    }

    /// <summary>
    /// Short descriptive label for the idea.
    /// </summary>
    [BsonElement("label")]
    public string Label
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Full written content of the idea.
    /// </summary>
    [BsonElement("content")]
    public string Content
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Tags used to categorize or classify the idea.
    /// </summary>
    [BsonElement("tags")]
    public List<string> Tags
    {
        get;
        set;
    } = [];

    /// <summary>
    /// UTC timestamp indicating when the idea was created.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp indicating the last time the idea was updated.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;

    /// <summary>
    /// Indicates whether the idea is marked as a favorite.
    /// </summary>
    [BsonElement("favorite")]
    public bool Favorite
    {
        get;
        set;
    } = false;
}
