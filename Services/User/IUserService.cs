using Nastaran_bot.Contracts.User;

namespace Nastaran_bot.Services.User;

public interface IUserService
{
    // ───────────────────────────────────────────────
    // Create / Get / Delete
    // ───────────────────────────────────────────────

    public Task<Models.User> AddUserAsync(
        long telegramId,
        string username,
        string firstName,
        string timezone = "UTC"
    );

    public Task<Models.User> GetUserByTelegramIdAsync(long telegramId);

    public Task<Models.User> GetUserByIdAsync(string id);

    public Task<bool> DeleteUserAsync(string id);

    // ───────────────────────────────────────────────
    // Updates (used directly by command handlers)
    // ───────────────────────────────────────────────

    public Task<Models.User> UpdateUserAsync(string id, UserUpdateDto update);

    public Task<Models.User> UpdateLocationAsync(string id, LocationDto location);

    public Task<Models.User> UpdateTimezoneAsync(string id, string timezone);

    public Task<Models.User> UpdatePreferencesAsync(string id, PreferencesDto prefs);

    // ───────────────────────────────────────────────
    // Favorite artists (used in Telegram commands)
    // ───────────────────────────────────────────────

    public Task<Models.User> AddFavoriteArtistAsync(string id, string artist);

    public Task<Models.User> RemoveFavoriteArtistAsync(string id, string artist);

    public Task<Models.User> ReplaceFavoriteArtistsAsync(string id, IEnumerable<string> artists);

    // ───────────────────────────────────────────────
    // Last checks (used by scheduled jobs or bot logic)
    // ───────────────────────────────────────────────

    public Task<Models.User> SetLastCheckAsync(
        string id,
        DateTime? spotify = null,
        DateTime? weather = null
    );
}
