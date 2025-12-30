namespace Core.Services.Idea;

/// <summary>
/// Provides high-level operations for creating, retrieving, and managing <see cref="Contracts.Models.Idea"/>.
/// </summary>
public interface IIdeaService
{
    /// <summary>
    /// Creates a new idea for the given Telegram user.
    /// </summary>
    /// <param name="telegramId">Telegram user identifier.</param>
    /// <param name="content">Text content of the idea.</param>
    /// <param name="label">Optional category label.</param>
    /// <param name="tags">Optional list of tags.</param>
    /// <param name="favorite">Whether the idea is marked as a favorite.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created idea.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown if content is null or whitespace.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if TelegramId is not a valid identifier (e.g., non-positive).
    /// </exception>
    public Task<Contracts.Models.Idea> AddIdeaAsync(
        long telegramId,
        string content,
        string label = "idea",
        List<string> tags = null,
        bool favorite = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns all ideas belonging to the given Telegram user as an asynchronous stream.
    /// </summary>
    /// <param name="telegramId">Telegram user identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>An asynchronous stream of ideas.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if TelegramId is not a valid identifier (e.g., non-positive).
    /// </exception>
    public IAsyncEnumerable<Contracts.Models.Idea> GetUserIdeasAsync(
        long telegramId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an idea by its identifier.
    /// </summary>
    /// <param name="id">Idea identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the idea was deleted.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if id is null or whitespace.
    /// </exception>
    public Task<bool> DeleteIdeaAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single idea by its identifier.
    /// </summary>
    /// <param name="id">Idea identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The idea or null.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if id is null or whitespace.
    /// </exception>
    public Task<Contracts.Models.Idea> GetIdeaByIdAsync(string id, CancellationToken cancellationToken = default);
}
