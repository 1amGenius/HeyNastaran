namespace Core.Contracts.Dtos.User;

/// <summary>
/// Data transfer object representing a user's last check for some content and preferences.
/// Mirrors the structure of <see cref="Models.LastCheck"/>.
/// </summary>
public sealed class LastCheckDto
{
    /// <summary>
    /// Timestamp of the last Spotify check for the user.
    /// </summary>
    public required DateTime Spotify
    {
        get;
        set;
    }

    /// <summary>
    /// Timestamp of the last weather check for the user.
    /// </summary>
    public required DateTime Weather
    {
        get;
        set;
    }
}
