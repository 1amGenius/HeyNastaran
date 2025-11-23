namespace Nastaran_bot.Contracts.User;

public class PreferencesDto
{
    public bool? DailyMusic
    {
        get;
        set;
    }

    public bool? DailyQuote
    {
        get;
        set;
    }
    
    public bool? WeatherUpdates
    {
        get;
        set;
    }
}
