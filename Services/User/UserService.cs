using Nastaran_bot.Models;
using Nastaran_bot.Repositories.User;

namespace Nastaran_bot.Services.User;

public class UserService(IUserRepository userRepository, ILogger<UserService> logger) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ILogger<UserService> _logger = logger;

    public async Task<Models.User> AddUserAsync(
        long telegramId,
        string username,
        string firstName,
        string timezone = "UTC")
    {
        try
        {
            var newUser = new Models.User
            {
                TelegramId = telegramId,
                Username = username,
                FirstName = firstName,
                Timezone = timezone,
                Location = new (), 
                FavoriteArtists = [],
                Preferences = new(),
                LastCheck = new (), 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _userRepository.CreateAsync(newUser);
            return newUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user for TelegramId {telegramId}", telegramId);
            throw;
        }
    }

    public async Task<IEnumerable<Models.User>> GetUsersAsync(long telegramId)
    {
        try
        {
            return await _userRepository.GetByTelegramIdAsync(telegramId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user for TelegramId {telegramId}", telegramId);
            return [];
        }
    }

    public async Task<bool> DeleteUserAsync(string id)
    {
        try
        {
            return await _userRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {id}", id);
            return false;
        }
    }

    public async Task<Models.User> GetUserByIdAsync(string id)
    {
        try
        {
            return await _userRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting note {id}", id);
            return null;
        }
    }
}
