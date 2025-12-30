using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Contracts.Models;

/// <summary>
/// Represents a stored inspiration item containing text, imagery, and metadata.
/// </summary>
public sealed class Inspiration
{
    /// <summary>
    /// Unique identifier for the inspiration document.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Telegram user identifier associated with the inspiration item.
    /// </summary>
    [BsonElement("telegramId")]
    public long TelegramId
    {
        get;
        set;
    }

    /// <summary>
    /// Short descriptive label for the inspiration item.
    /// </summary>
    [BsonElement("label")]
    public string Label
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Identifier of the image file stored on Telegram.
    /// </summary>
    [BsonElement("imageFileId")]
    public string ImageFileId
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Main text content describing the inspiration.
    /// </summary>
    [BsonElement("content")]
    public string Content
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Tags used to categorize or classify the inspiration item.
    /// </summary>
    [BsonElement("tags")]
    public List<string> Tags
    {
        get;
        set;
    } = [];

    /// <summary>
    /// UTC timestamp for when the item was created.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp for the last time the item was updated.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;

    /// <summary>
    /// Indicates whether the inspiration item is marked as a favorite.
    /// </summary>
    [BsonElement("favorite")]
    public bool Favorite
    {
        get;
        set;
    } = false;
}
