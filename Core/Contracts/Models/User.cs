using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Contracts.Models;

/// <summary>
/// Represents an application user with profile data, preferences, and metadata.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Unique identifier for the user document.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Telegram user identifier linked to this profile.
    /// </summary>
    [BsonElement("telegramId")]
    public long TelegramId
    {
        get;
        set;
    }

    /// <summary>
    /// Telegram username of the user.
    /// </summary>
    [BsonElement("username")]
    public string Username
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// First name of the user.
    /// </summary>
    [BsonElement("firstName")]
    public string FirstName
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Geographic location information for the user.
    /// </summary>
    [BsonElement("location")]
    public Location Location
    {
        get;
        set;
    } = new();

    /// <summary>
    /// Indicates whether the user is currently searching for a city.
    /// </summary>
    [BsonElement("isSearchingCity")]
    public bool IsSearchingCity 
    {
        get; 
        set; 
    } = false;

    /// <summary>
    /// Timezone identifier associated with the user.
    /// </summary>
    [BsonElement("timezone")]
    public string Timezone
    {
        get;
        set;
    } = "UTC";

    /// <summary>
    /// List of the user's favorite music artists.
    /// </summary>
    [BsonElement("favoriteArtists")]
    public List<string> FavoriteArtists
    {
        get;
        set;
    } = [];

    /// <summary>
    /// User-defined content and notification preferences.
    /// </summary>
    [BsonElement("preferences")]
    public Preferences Preferences
    {
        get;
        set;
    } = new();

    /// <summary>
    /// Tracks timestamps of the user's last automated activity checks.
    /// </summary>
    [BsonElement("lastCheck")]
    public LastCheck LastCheck
    {
        get;
        set;
    } = new();

    /// <summary>
    /// UTC timestamp representing when the user profile was created.
    /// </summary>
    [BsonElement("createdAt")]
    public DateTime CreatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;

    /// <summary>
    /// UTC timestamp representing when the user profile was last updated.
    /// </summary>
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt
    {
        get;
        set;
    } = DateTime.UtcNow;
}

/// <summary>
/// Represents a user's geographic location data.
/// </summary>
public class Location
{
    /// <summary>
    /// City name of the user's location.
    /// </summary>
    public string City
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Country name of the user's location.
    /// </summary>
    public string Country
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Latitude coordinate of the user's location.
    /// </summary>
    public double Lat
    {
        get;
        set;
    }

    /// <summary>
    /// Longitude coordinate of the user's location.
    /// </summary>
    public double Lon
    {
        get;
        set;
    }
}

/// <summary>
/// Represents the user's configurable preferences for notifications and daily content.
/// </summary>
public class Preferences
{
    /// <summary>
    /// Indicates whether the user receives a daily music recommendation.
    /// </summary>
    public bool DailyMusic
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Indicates whether the user receives a daily quote.
    /// </summary>
    public bool DailyQuote
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Indicates whether the user receives weather updates.
    /// </summary>
    public bool WeatherUpdates
    {
        get;
        set;
    } = true;
}

/// <summary>
/// Tracks the last time automated checks were performed for various services.
/// </summary>
public class LastCheck
{
    /// <summary>
    /// Timestamp of the last Spotify check for the user.
    /// </summary>
    public DateTime Spotify
    {
        get;
        set;
    } = DateTime.MinValue;

    /// <summary>
    /// Timestamp of the last weather check for the user.
    /// </summary>
    public DateTime Weather
    {
        get;
        set;
    } = DateTime.MinValue;
}
