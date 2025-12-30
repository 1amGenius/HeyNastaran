namespace Core.Contracts.Dtos.User;

/// <summary>
/// Data transfer object representing a user's geographic location.
/// Mirrors the structure of <see cref="Models.Location"/> for external requests.
/// </summary>
public sealed class LocationDto
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
    public required double Lat
    {
        get;
        set;
    }

    /// <summary>
    /// Longitude coordinate of the user's location.
    /// </summary>
    public required double Lon
    {
        get; 
        set;
    }
}
