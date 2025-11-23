using System.Reflection.Emit;

using Nastaran_bot.Contracts.User;
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
                Location = new Models.Location(),
                FavoriteArtists = [],
                Preferences = new(),
                LastCheck = new(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
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

    public async Task<IEnumerable<Models.User>> GetUsersAsync(
        long? telegramId = null,
        int? page = null,
        int? pageSize = null)
    {
        try
        {
            if (telegramId.HasValue)
            {
                return await _userRepository.GetByTelegramIdAsync(telegramId.Value);
            }

            IEnumerable<Models.User> all = await _userRepository.GetAllAsync();
            return page.HasValue && pageSize.HasValue ? all.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value) : all;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users");
            return [];
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
            _logger.LogError(ex, "Error getting user {id}", id);
            return null;
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

    public async Task<Models.User> UpdateUserAsync(string id, UserUpdateDto update)
    {
        try
        {
            Models.User user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            if (update.Username != null)
            {
                user.Username = update.Username;
            }

            if (update.FirstName != null)
            {
                user.FirstName = update.FirstName;
            }

            if (update.Timezone != null)
            {
                user.Timezone = update.Timezone;
            }

            if (update.Location != null)
            {
                user.Location = new Models.Location
                {
                    City = update.Location.City,
                    Country = update.Location.Country,
                    Lat = update.Location.Lat,
                    Lon = update.Location.Lon
                };
            }
            
            if (update.Preferences != null)
            {
                user.Preferences = new Models.Preferences
                {
                    DailyMusic = update.Preferences.DailyMusic ?? user.Preferences.DailyMusic,
                    DailyQuote = update.Preferences.DailyQuote ?? user.Preferences.DailyQuote,
                    WeatherUpdates = update.Preferences.WeatherUpdates ?? user.Preferences.WeatherUpdates
                };
            }

            if (update.FavoriteArtists != null)
            {
                user.FavoriteArtists = [.. update.FavoriteArtists];
            }

            user.UpdatedAt = DateTime.UtcNow;
            
            await _userRepository.UpdateAsync(user);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {id}", id);
            throw;
        }
    }

    public async Task<Models.User> UpdateLocationAsync(string id, LocationDto location)
    {
        Models.User user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.Location = new Models.Location
        {
            City = location.City,
            Country = location.Country,
            Lat = location.Lat,
            Lon = location.Lon
        };

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<Models.User> UpdateTimezoneAsync(string id, string timezone)
    {
        Models.User user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.Timezone = timezone;
     
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<Models.User> UpdatePreferencesAsync(string id, PreferencesDto prefs)
    {
        Models.User user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.Preferences.DailyMusic = prefs.DailyMusic ?? user.Preferences.DailyMusic;
        user.Preferences.DailyQuote = prefs.DailyQuote ?? user.Preferences.DailyQuote;
        user.Preferences.WeatherUpdates = prefs.WeatherUpdates ?? user.Preferences.WeatherUpdates;

        user.UpdatedAt = DateTime.UtcNow;
     
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<Models.User> AddFavoriteArtistAsync(string id, string artist)
    {
        Models.User user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.FavoriteArtists ??= [];

        if (!user.FavoriteArtists.Contains(artist, StringComparer.OrdinalIgnoreCase))
        {
            user.FavoriteArtists.Add(artist);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        return user;
    }

    public async Task<Models.User> RemoveFavoriteArtistAsync(string id, string artist)
    {
        Models.User user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        if (user.FavoriteArtists?.RemoveAll(a => a.Equals(artist, StringComparison.OrdinalIgnoreCase)) > 0)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
        }

        return user;
    }

    public async Task<Models.User> ReplaceFavoriteArtistsAsync(string id, IEnumerable<string> artists)
    {
        Models.User user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        user.FavoriteArtists = [.. artists];
        
        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<Models.User> SetLastCheckAsync(
        string id,
        DateTime? spotify = null,
        DateTime? weather = null)
    {
        Models.User user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return null;
        }

        if (spotify.HasValue)
        {
            user.LastCheck.Spotify = spotify.Value;
        }

        if (weather.HasValue)
        {
            user.LastCheck.Weather = weather.Value;
        }

        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user);
        return user;
    }

    // todo: move to repository-level filters to improve speed later
    public async Task<IEnumerable<Models.User>> GetUsersForNotificationAsync(
        string notificationType,
        string timezone = null,
        int? limit = null)
    {
        IEnumerable<Models.User> all = await _userRepository.GetAllAsync();
        IEnumerable<Models.User> filtered = notificationType switch
        {
            "DailyMusic" => all.Where(u => u.Preferences?.DailyMusic ?? false),
            "DailyQuote" => all.Where(u => u.Preferences?.DailyQuote ?? false),
            "WeatherUpdates" => all.Where(u => u.Preferences?.WeatherUpdates ?? false),
            _ => []
        };

        if (!string.IsNullOrEmpty(timezone))
        {
            filtered = filtered.Where(u => string.Equals(u.Timezone, timezone, StringComparison.OrdinalIgnoreCase));
        }

        if (limit.HasValue)
        {
            filtered = filtered.Take(limit.Value);
        }

        return filtered;
    }

    public async Task<IEnumerable<Models.User>> GetUsersNeedingSpotifyCheckAsync(TimeSpan threshold, int? limit = null)
    {
        IEnumerable<Models.User> all = await _userRepository.GetAllAsync();
        
        DateTime cutoff = DateTime.UtcNow - threshold;
        
        IEnumerable<Models.User> filtered = all.Where(u => u.Preferences?.DailyMusic ?? false)
            .Where(u => u.LastCheck?.Spotify == DateTime.MinValue || u.LastCheck.Spotify <= cutoff);

        if (limit.HasValue)
        {
            filtered = filtered.Take(limit.Value);
        }

        return filtered;
    }

    public async Task<IEnumerable<Models.User>> GetUsersNeedingWeatherCheckAsync(TimeSpan threshold, int? limit = null)
    {
        IEnumerable<Models.User> all = await _userRepository.GetAllAsync();
        
        DateTime cutoff = DateTime.UtcNow - threshold;
        
        IEnumerable<Models.User> filtered = all.Where(u => u.Preferences?.WeatherUpdates ?? false)
            .Where(u => u.LastCheck?.Weather == DateTime.MinValue || u.LastCheck.Weather <= cutoff);

        if (limit.HasValue)
        {
            filtered = filtered.Take(limit.Value);
        }

        return filtered;
    }
}
