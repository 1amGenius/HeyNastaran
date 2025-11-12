namespace Nastaran_bot.Services.User;

public interface IUserService
{
    public Task<Models.User> AddUserAsync(
        long telegramId,
        string username,
        string firstName,
        string timezone = "UTC");
    public Task<IEnumerable<Models.User>> GetUsersAsync(long telegramId);
    public Task<bool> DeleteUserAsync(string id);
    public Task<Models.User> GetUserByIdAsync(string id);
}
