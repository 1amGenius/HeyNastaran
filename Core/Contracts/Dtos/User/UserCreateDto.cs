namespace Core.Contracts.Dtos.User;

/// <summary>
/// Data transfer object used for creating a new user profile.
/// Provides initial Telegram identity, basic profile information, and optional location data.
/// Corresponds to core fields in <see cref="Models.User"/>.
/// </summary>
public sealed class UserCreateDto
{
    /// <summary>
    /// Telegram user identifier linked to the profile being created.
    /// </summary>
    public required long TelegramId
    {
        get;
        set;
    }

    /// <summary>
    /// Telegram username of the user.
    /// </summary>
    public required string Username 
    { 
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// First name of the user.
    /// </summary>
    public required string FirstName 
    { 
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Timezone identifier for the user. Defaults to UTC.
    /// </summary>
    public string Timezone 
    { 
        get;
        set;
    } = "UTC";

    /// <summary>
    /// Optional geographic location information for the user.
    /// </summary>
    public LocationDto Location
    {
        get;
        set;
    }
}
