namespace Nastaran_bot.Contracts.User;

/// <summary>
/// Data transfer object used for updating an existing user profile.
/// All fields are optional; only provided values will be applied.
/// Maps to editable aspects of <see cref="Models.User"/>.
/// </summary>
public class UserUpdateDto
{
    /// <summary>
    /// Updated Telegram username of the user.
    /// </summary>
    public string Username
    {
        get; 
        set;
    }

    /// <summary>
    /// Updated first name of the user.
    /// </summary>
    public string FirstName
    {
        get; 
        set;
    }

    /// <summary>
    /// Updated timezone identifier for the user.
    /// </summary>
    public string Timezone
    {
        get; 
        set;
    }

    /// <summary>
    /// Updated geographic location information for the user.
    /// </summary>
    public LocationDto Location
    {
        get; 
        set;
    }

    /// <summary>
    /// Updated user-configured preferences.
    /// </summary>
    public PreferencesDto Preferences
    {
        get; 
        set;
    }

    /// <summary>
    /// Updated list of the user's favorite music artists.
    /// </summary>
    public List<string> FavoriteArtists
    {
        get; 
        set;
    }

    /// <summary>
    /// Indicates whether the user is currently searching for a city.
    /// Nullable to allow selective updates.
    /// </summary>
    public bool? IsSearchingCity
    {
        get; 
        set;
    }
}
