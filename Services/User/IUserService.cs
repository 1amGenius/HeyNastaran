using Nastaran_bot.Contracts.User;

namespace Nastaran_bot.Services.User;

public interface IUserService
{
    public Task<Models.User> AddUserAsync(
        long telegramId,
        string username,
        string firstName,
        string timezone = "UTC"
    );

    public Task<IEnumerable<Models.User>> GetUsersAsync(
        long? telegramId = null,
        int? page = null,
        int? pageSize = null
    );
    
    public Task<Models.User> GetUserByIdAsync(string id);
    
    public Task<bool> DeleteUserAsync(string id);

    // Updates
    public Task<Models.User> UpdateUserAsync(string id, UserUpdateDto update);

    public Task<Models.User> UpdateLocationAsync(string id, LocationDto location);

    public Task<Models.User> UpdateTimezoneAsync(string id, string timezone);

    public Task<Models.User> UpdatePreferencesAsync(string id, PreferencesDto prefs);

    public Task<Models.User> AddFavoriteArtistAsync(string id, string artist);

    public Task<Models.User> RemoveFavoriteArtistAsync(string id, string artist);

    public Task<Models.User> ReplaceFavoriteArtistsAsync(string id, IEnumerable<string> artists);

    // LastCheck
    public Task<Models.User> SetLastCheckAsync(
        string id,
        DateTime? spotify = null,
        DateTime? weather = null
    );

    // Notification queries
    public Task<IEnumerable<Models.User>> GetUsersForNotificationAsync(
        string notificationType, // "DailyMusic" | "DailyQuote" | "WeatherUpdates"
        string timezone = null,
        int? limit = null
    );

    // helpers
    public Task<IEnumerable<Models.User>> GetUsersNeedingSpotifyCheckAsync(TimeSpan threshold, int? limit = null);
    public Task<IEnumerable<Models.User>> GetUsersNeedingWeatherCheckAsync(TimeSpan threshold, int? limit = null);
}
