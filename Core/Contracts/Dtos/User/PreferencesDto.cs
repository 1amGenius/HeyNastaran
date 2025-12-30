namespace Core.Contracts.Dtos.User;

/// <summary>
/// Data transfer object representing a user's configurable content and notification preferences.
/// Mirrors the structure of <see cref="Models.Preferences"/>.
/// </summary>
public sealed class PreferencesDto
{
    /// <summary>
    /// Indicates whether the user receives a daily music recommendation.
    /// </summary>
    public bool DailyMusic
    {
        get;
        set;
    }

    /// <summary>
    /// Indicates whether the user receives a daily quote.
    /// </summary>
    public bool DailyQuote
    {
        get; 
        set;
    }

    /// <summary>
    /// Indicates whether the user receives weather updates.
    /// </summary>
    public bool WeatherUpdates
    {
        get;
        set;
    }
}
