namespace Nastaran_bot.Contracts.User;

public class LocationDto
{
    public string City 
    {
        get;
        set;
    } = string.Empty;
    
    public string Country 
    { 
        get; 
        set;
    } = string.Empty;
    
    public double Lat
    {
        get;
        set;
    }

    public double Lon
    {
        get; 
        set;
    }
}
