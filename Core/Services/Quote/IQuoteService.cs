namespace Core.Services.Quote;

/// <summary>
/// Provides high-level operations for creating, retrieving, and managing
/// user-submitted <see cref="Contracts.Models.Quote"/> items.
/// </summary>
public interface IQuoteService
{
    /// <summary>
    /// Creates a new quote entry for the specified Telegram user.
    /// </summary>
    /// <param name="telegramId">Identifier of the user creating the inspiration.</param>
    /// <param name="text">The content of the quote</param>
    /// <param name="category">Optional / The genre of the quote</param>
    /// <param name="author">Optional / Who wrote this quote</param>
    /// <returns>The created <see cref="Contracts.Models.Quote"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if telegram ID is not a valid identifier (e.g., non-positive).
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the text is null, empty, or whitespace.
    /// </exception>
    public Task<Contracts.Models.Quote> AddQuoteAsync(
        long telegramId,
        string text,
        string category = "general",
        string author = "unknown",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all quotes created by the specified Telegram user.
    /// </summary>
    /// <param name="telegramId">Identifier of the user whose quotes will be returned.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A sequence of quotes for the user.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if telegram ID is not a valid identifier (e.g., non-positive).
    /// </exception>
    public IAsyncEnumerable<Contracts.Models.Quote> GetUserQuotesAsync(long telegramId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an quote entry by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the quote to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the item was deleted; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the quote ID is null, empty, or whitespace.
    /// </exception>
    public Task<bool> DeleteQuoteAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single quote item by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the quote to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The matching <see cref="Contracts.Models.Quote"/> instance,
    /// or <c>null</c> if not found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the quote ID is null, empty, or whitespace.
    /// </exception>
    public Task<Contracts.Models.Quote> GetQuoteByIdAsync(string id, CancellationToken cancellationToken);
}
