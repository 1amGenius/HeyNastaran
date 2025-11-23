namespace Nastaran_bot.Contracts.User;

public sealed class UserCreateDto
{
    public long TelegramId
    {
        get;
        set;
    }

    public string Username 
    { 
        get; 
        set; 
    } = string.Empty;
    
    public string FirstName 
    {
        get;
        set;
    } = string.Empty;
    
    public string Timezone 
    {
        get;
        set;
    } = "UTC";
    
    public LocationDto Location 
    {
        get;
        set; 
    } = null;
}
