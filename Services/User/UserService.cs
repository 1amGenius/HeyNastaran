using Nastaran_bot.Contracts.User;
using Nastaran_bot.Repositories.User;

namespace Nastaran_bot.Services.User;

/// <summary>
/// Provides the concrete implementation of <see cref="IUserService"/>,
/// encapsulating all user-related business logic and persistence orchestration.
/// </summary>
/// <remarks>
/// This service acts as the authoritative boundary for user state mutations.
/// It coordinates validation, normalization, and update timestamps while
/// delegating persistence concerns to <see cref="IUserRepository"/>.
/// </remarks>
public class UserService(IUserRepository userRepository) : IUserService
{
    /// <summary>
    /// Repository responsible for user persistence and retrieval operations.
    /// </summary>
    private readonly IUserRepository _userRepository = userRepository;

    /// <inheritdoc />
    public async Task<Models.User> AddUserAsync(
        long telegramId,
        string username,
        string firstName,
        string timezone = "UTC",
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);

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

        await _userRepository.AddAsync(newUser, cancellationToken).ConfigureAwait(false);
        return newUser;
    }

    /// <inheritdoc />
    public async Task<Models.User> GetUserByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId);

        IAsyncEnumerable<Models.User> users = _userRepository.GetByTelegramIdAsync(telegramId, cancellationToken);
        return await users.FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Models.User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id))
            : await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id))
            : await _userRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Models.User> UpdateUserAsync(string id, UserUpdateDto update, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(update);

        Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        if (update.Username is not null)
        {
            user.Username = update.Username;
        }

        if (update.FirstName is not null)
        {
            user.FirstName = update.FirstName;
        }

        if (update.Timezone is not null)
        {
            user.Timezone = update.Timezone;
        }

        if (update.Location is not null)
        {
            user.Location = new Models.Location
            {
                City = update.Location.City,
                Country = update.Location.Country,
                Lat = update.Location.Lat,
                Lon = update.Location.Lon
            };
        }

        if (update.Preferences is not null)
        {
            user.Preferences = new Models.Preferences
            {
                DailyMusic = update.Preferences.DailyMusic,
                DailyQuote = update.Preferences.DailyQuote,
                WeatherUpdates = update.Preferences.WeatherUpdates
            };
        }

        if (update.FavoriteArtists is not null)
        {
            user.FavoriteArtists = [.. update.FavoriteArtists];
        }

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return user;
    }

    /// <inheritdoc />
    public async Task<Models.User> UpdateLocationAsync(string id, LocationDto location, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(location);

        Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
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
        
        await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return user;
    }

    /// <inheritdoc />
    public async Task<Models.User> UpdateTimezoneAsync(string id, string timezone, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(timezone);

        Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        user.Timezone = timezone;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return user;
    }

    /// <inheritdoc />
    public async Task<Models.User> UpdatePreferencesAsync(string id, PreferencesDto prefs, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(prefs);

        Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        user.Preferences.DailyMusic = prefs.DailyMusic;
        user.Preferences.DailyQuote = prefs.DailyQuote;
        user.Preferences.WeatherUpdates = prefs.WeatherUpdates;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user).ConfigureAwait(false);
        return user;
    }

    /// <inheritdoc />
    public async Task<Models.User> AddFavoriteArtistAsync(string id, string artist, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(artist);

        Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        user.FavoriteArtists ??= [];

        if (!user.FavoriteArtists.Contains(artist, StringComparer.OrdinalIgnoreCase))
        {
            user.FavoriteArtists.Add(artist);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        }

        return user;
    }

    /// <inheritdoc />
    public async Task<Models.User> RemoveFavoriteArtistAsync(string id, string artist, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(artist);

        Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        if (user.FavoriteArtists?.RemoveAll(a => a.Equals(artist, StringComparison.OrdinalIgnoreCase)) > 0)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        }

        return user;
    }

    /// <inheritdoc />
    public async Task<Models.User> ReplaceFavoriteArtistsAsync(string id, IEnumerable<string> artists, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);
        ArgumentNullException.ThrowIfNull(artists);

        Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        user.FavoriteArtists = [.. artists];

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return user;
    }

    /// <inheritdoc />
    public async Task<Models.User> SetLastCheckAsync(
        string id,
        DateTime? spotify = null,
        DateTime? weather = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
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

        await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return user;
    }
}
