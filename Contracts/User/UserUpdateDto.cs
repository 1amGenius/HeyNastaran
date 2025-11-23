namespace Nastaran_bot.Contracts.User;

public class UserUpdateDto
{
    public string Username
    {
        get; 
        set;
    }

    public string FirstName
    {
        get;
        set;
    }
    
    public string Timezone
    {
        get; 
        set;
    }
    
    public LocationDto Location
    {
        get;
        set;
    }
    
    public PreferencesDto Preferences
    {
        get;
        set;
    }
    
    public List<string> FavoriteArtists
    {
        get; 
        set;
    }
}
