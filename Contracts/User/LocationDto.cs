namespace Nastaran_bot.Contracts.User;

/// <summary>
/// Data transfer object representing a user's geographic location.
/// Mirrors the structure of <see cref="Models.Location"/> for external requests.
/// </summary>
public class LocationDto
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
