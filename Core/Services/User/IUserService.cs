using Core.Contracts.Dtos.User;

namespace Core.Services.User;

/// <summary>
/// Defines the contract for managing application users, including creation,
/// retrieval, updates, preferences, and domain-specific metadata.
/// </summary>
/// <remarks>
/// This service acts as the primary orchestration layer between command handlers,
/// scheduled jobs, and the persistence layer. Implementations are expected to handle
/// validation, normalization, and consistency of user state.
/// </remarks>
public interface IUserService
{
    // ───────────────────────────────────────────────
    // Create / Get / Delete
    // ───────────────────────────────────────────────

    /// <summary>
    /// Creates and persists a new user based on Telegram identity data.
    /// </summary>
    /// <param name="telegramId">Unique Telegram user identifier.</param>
    /// <param name="username">Telegram username (may be null or empty).</param>
    /// <param name="firstName">User's first name as provided by Telegram.</param>
    /// <param name="timezone">
    /// IANA timezone identifier for the user.
    /// Defaults to <c>UTC</c> if not provided.
    /// </param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The newly created <see cref="Contracts.Models.User"/> instance.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the users' Telegram ID is invalid (e.g., non-positive).
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if username or first name are invalid (e.g., empty).
    /// </exception>
    public Task<Contracts.Models.User> AddUserAsync(
        long telegramId,
        string username,
        string firstName,
        string timezone = "UTC",
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a user by their Telegram identifier.
    /// </summary>
    /// <param name="telegramId">Telegram user identifier.</param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The matching <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the provided Telegram ID is invalid (e.g., non-positive).
    /// </exception>
    public Task<Contracts.Models.User> GetUserByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their internal system identifier.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The matching <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided user ID is null, empty, or whitespace.
    /// </exception>
    public Task<Contracts.Models.User> GetUserByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user and all associated data.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// <c>true</c> if the user was deleted; otherwise <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided user ID is null, empty, or whitespace.
    /// </exception>
    public Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken = default);

    // ───────────────────────────────────────────────
    // Updates (used directly by command handlers)
    // ───────────────────────────────────────────────

    /// <summary>
    /// Applies a partial update to a user's profile and related sub-resources.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="update">
    /// DTO containing the fields to update. Null values are ignored.
    /// </param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The updated <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided user ID is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the update DTO is null.
    /// </exception>
    public Task<Contracts.Models.User> UpdateUserAsync(string id, UserUpdateDto update, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's geographic location.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="location">New location data.</param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The updated <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided user ID is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the location DTO is null.
    /// </exception>
    public Task<Contracts.Models.User> UpdateLocationAsync(string id, LocationDto location, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the user's timezone.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="timezone">IANA timezone identifier.</param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The updated <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided user ID or timezone is null, empty, or whitespace.
    /// </exception>
    public Task<Contracts.Models.User> UpdateTimezoneAsync(string id, string timezone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates user notification and content preferences.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="prefs">
    /// DTO containing preference values. Null values preserve existing settings.
    /// </param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The updated <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided user ID is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the preferences DTO is null.
    /// </exception>
    public Task<Contracts.Models.User> UpdatePreferencesAsync(string id, PreferencesDto prefs, CancellationToken cancellationToken = default);

    // ───────────────────────────────────────────────
    // Favorite artists (used in Telegram commands)
    // ───────────────────────────────────────────────

    /// <summary>
    /// Adds an artist to the user's list of favorite artists.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="artist">Artist name to add.</param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The updated <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Artist matching is case-insensitive. Duplicate entries are ignored.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided user ID or artist name is null, empty, or whitespace.
    /// </exception>
    public Task<Contracts.Models.User> AddFavoriteArtistAsync(string id, string artist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an artist from the user's list of favorite artists.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="artist">Artist name to remove.</param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The updated <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided user ID or artist name is null, empty, or whitespace.
    /// </exception>
    public Task<Contracts.Models.User> RemoveFavoriteArtistAsync(string id, string artist, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the user's favorite artists with a new collection.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="artists">Complete list of favorite artists.</param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The updated <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided user ID is null, empty, or whitespace.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the artists collection is null.
    /// </exception>
    public Task<Contracts.Models.User> ReplaceFavoriteArtistsAsync(string id, IEnumerable<string> artists, CancellationToken cancellationToken = default);

    // ───────────────────────────────────────────────
    // Last checks (used by scheduled jobs or bot logic)
    // ───────────────────────────────────────────────

    /// <summary>
    /// Updates timestamps indicating the last successful external data checks.
    /// </summary>
    /// <param name="id">Internal user identifier.</param>
    /// <param name="spotify">
    /// Timestamp of the last Spotify-related check, or <c>null</c> to leave unchanged.
    /// </param>
    /// <param name="weather">
    /// Timestamp of the last weather-related check, or <c>null</c> to leave unchanged.
    /// </param>
    /// <param name="cancellationToken">CancelationToken</param>
    /// <returns>
    /// The updated <see cref="Contracts.Models.User"/> if found; otherwise <c>null</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the provided user ID is null, empty, or whitespace.
    /// </exception>
    public Task<Contracts.Models.User> SetLastCheckAsync(
        string id,
        LastCheckDto lastCheck,
        CancellationToken cancellationToken = default
    );
}
