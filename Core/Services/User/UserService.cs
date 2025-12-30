using Core.Contracts.Dtos.User;
using Core.Repositories.User;
using Core.Utils.Mappers;

namespace Core.Services.User;

/// <summary>
/// Provides the concrete implementation of <see cref="IUserService"/>,
/// encapsulating all user-related business logic and persistence orchestration.
/// </summary>
/// <remarks>
/// This service acts as the authoritative boundary for user state mutations.
/// It coordinates validation, normalization, and update timestamps while
/// delegating persistence concerns to <see cref="IUserRepository"/>.
/// </remarks>
public sealed class UserService(IUserRepository userRepository) : IUserService
{
    /// <summary>
    /// Repository responsible for user persistence and retrieval operations.
    /// </summary>
    private readonly IUserRepository _userRepository = userRepository;

    /// <inheritdoc />
    /// <remarks>Insert the default (null mostly) values for Location, Prefrences, Last check</remarks>
    public async Task<Contracts.Models.User> AddUserAsync(
        long telegramId,
        string username,
        string firstName,
        string timezone = "UTC",
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId, nameof(telegramId));
        ArgumentException.ThrowIfNullOrWhiteSpace(username, nameof(username));
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));

        var newUser = new Contracts.Models.User
        {
            TelegramId = telegramId,
            Username = username,
            FirstName = firstName,
            Timezone = timezone,
            Location = new(),
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
    public async Task<Contracts.Models.User> GetUserByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId, nameof(telegramId));

        IAsyncEnumerable<Contracts.Models.User> users = _userRepository.GetByTelegramIdAsync(telegramId, cancellationToken);
        return await users.LastOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Contracts.Models.User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "User ID cannot be null or empty")
            : await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "User ID cannot be null or empty")
            : await _userRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Contracts.Models.User> UpdateUserAsync(string id, UserUpdateDto update, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentNullException.ThrowIfNull(update, nameof(update));

        Contracts.Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
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

        if (update.IsSearchingCity.HasValue is true)
        {
            user.IsSearchingCity = update.IsSearchingCity.Value;
        }

        if (update.Location is not null)
        {
            user.Location = LocationMapper.ToModel(update.Location);
        }

        if (update.Preferences is not null)
        {
            user.Preferences = PreferencesMapper.ToModel(update.Preferences);
        }

        if (update.LastCkeck is not null)
        {
            user.LastCheck = LastCheckMapper.ToModel(update.LastCkeck);
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
    public async Task<Contracts.Models.User> UpdateLocationAsync(string id, LocationDto location, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentNullException.ThrowIfNull(location, nameof(location));

        Contracts.Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        user.Location = LocationMapper.ToModel(location);

        user.UpdatedAt = DateTime.UtcNow;
        
        await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return user;
    }

    /// <inheritdoc />
    public async Task<Contracts.Models.User> UpdateTimezoneAsync(string id, string timezone, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(timezone, nameof(timezone));

        Contracts.Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
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
    public async Task<Contracts.Models.User> UpdatePreferencesAsync(string id, PreferencesDto prefs, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentNullException.ThrowIfNull(prefs, nameof(prefs));

        Contracts.Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        user.Preferences = PreferencesMapper.ToModel(prefs);

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return user;
    }

    /// <inheritdoc />
    public async Task<Contracts.Models.User> AddFavoriteArtistAsync(string id, string artist, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(artist, nameof(artist));

        Contracts.Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
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
    public async Task<Contracts.Models.User> RemoveFavoriteArtistAsync(string id, string artist, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(artist, nameof(artist));

        Contracts.Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
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
    public async Task<Contracts.Models.User> ReplaceFavoriteArtistsAsync(string id, IEnumerable<string> artists, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));
        ArgumentNullException.ThrowIfNull(artists, nameof(artists));

        Contracts.Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
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
    public async Task<Contracts.Models.User> SetLastCheckAsync(
        string id,
        LastCheckDto lastCheck,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        Contracts.Models.User user = await _userRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (user is null)
        {
            return null;
        }

        user.LastCheck = LastCheckMapper.ToModel(lastCheck);

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user, cancellationToken).ConfigureAwait(false);
        return user;
    }
}
