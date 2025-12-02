namespace Nastaran_bot.Contracts.Weather;

/// <summary>
/// Represents the result of a reverse-geocoding lookup, containing
/// resolved city and country information derived from geographic coordinates.
/// </summary>
public record ReverseGeocodingResult(
    /// <summary>
    /// Resolved city name for the provided coordinates.
    /// </summary>
    string City,
    
    /// <summary>
    /// Resolved country name for the provided coordinates.
    /// </summary>
    string Country
);

