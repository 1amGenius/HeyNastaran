using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nastaran_bot.Models;

public class Users
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("telegramId")]
    public long TelegramId { get; set; }

    [BsonElement("username")]
    public string Username { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; }

    [BsonElement("location")]
    public Location Location { get; set; }

    [BsonElement("timezone")]
    public string Timezone { get; set; }

    [BsonElement("favoriteArtists")]
    public List<string> FavoriteArtists { get; set; }

    [BsonElement("preferences")]
    public Preferences Preferences { get; set; }

    [BsonElement("lastCheck")]
    public LastCheck LastCheck { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public class Location
{
    public string City { get; set; }
    public string Country { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
}

public class Preferences
{
    public bool DailyMusic { get; set; }
    public bool DailyQuote { get; set; }
    public bool WeatherUpdates { get; set; }
}

public class LastCheck
{
    public DateTime Spotify { get; set; }
    public DateTime Weather { get; set; }
}
