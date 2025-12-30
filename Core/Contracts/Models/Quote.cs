using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Contracts.Models;

/// <summary>
/// Represents a stored quote with metadata, authorship, and categorization.
/// </summary>
public sealed class Quote
{
    /// <summary>
    /// Unique identifier for the quote document.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Telegram user identifier associated with the quote.
    /// </summary>
    [BsonElement("telegramId")]
    public long TelegramId
    {
        get;
        set;
    }

    /// <summary>
    /// Date string related to the quote, such as when it was noted or relevant.
    /// </summary>
    [BsonElement("date")]
    public string Date
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Full text of the quote.
    /// </summary>
    [BsonElement("text")]
    public string Text
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Category or thematic grouping of the quote.
    /// </summary>
    [BsonElement("category")]
    public string Category
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Author of the quote. Defaults to "unknown" if not provided.
    /// </summary>
    [BsonElement("author")]
    public string Author
    {
        get;
        set;
    } = "unknown";

    /// <summary>
    /// Timestamp indicating the last time this quote was used. Null if unused.
    /// </summary>
    [BsonElement("usedAt")]
    [BsonIgnoreIfNull]
    public DateTime? UsedAt
    {
        get; set;
    }

    /// <summary>
    /// UTC timestamp indicating when the quote was created.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp indicating when the quote was last updated.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;
}
