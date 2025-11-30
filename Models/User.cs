using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nastaran_bot.Models;

public class User
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

    [BsonElement("username")]
    public string Username
    {
        get;
        set;
    } = string.Empty;

    [BsonElement("firstName")]
    public string FirstName
    {
        get;
        set;
    } = string.Empty;

    [BsonElement("location")]
    public Location Location
    {
        get;
        set;
    } = new();

    [BsonElement("isSearchingCity")]
    public bool IsSearchingCity { get; set; } = false;

    [BsonElement("timezone")]
    public string Timezone
    {
        get;
        set;
    } = "UTC";

    [BsonElement("favoriteArtists")]
    public List<string> FavoriteArtists
    {
        get;
        set;
    } = [];

    [BsonElement("preferences")]
    public Preferences Preferences
    {
        get;
        set;
    } = new();

    [BsonElement("lastCheck")]
    public LastCheck LastCheck
    {
        get;
        set;
    } = new();

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

public class Location
{
    public string City
    {
        get;
        set;
    } = string.Empty;

    public string Country
    {
        get;
        set;
    } = string.Empty;

    public double Lat
    {
        get;
        set;
    }

    public double Lon
    {
        get;
        set;
    }
}

public class Preferences
{
    public bool DailyMusic
    {
        get;
        set;
    } = true;

    public bool DailyQuote
    {
        get;
        set;
    } = true;

    public bool WeatherUpdates
    {
        get;
        set;
    } = true;
}

public class LastCheck
{
    public DateTime Spotify
    {
        get;
        set;
    } = DateTime.MinValue;

    public DateTime Weather
    {
        get;
        set;
    } = DateTime.MinValue;
}
