namespace Nastaran_bot.Services.Quote;

/// <summary>
/// Provides high-level operations for creating, retrieving, and managing
/// user-submitted <see cref="Models.Quote"/> items.
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
    /// <returns>The created <see cref="Models.Quote"/> instance.</returns>
    public Task<Models.Quote> AddQuoteAsync(
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
    public IAsyncEnumerable<Models.Quote> GetUserQuotesAsync(long telegramId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an quote entry by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the quote to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the item was deleted; otherwise <c>false</c>.</returns>
    public Task<bool> DeleteQuoteAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a single quote item by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the quote to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The matching <see cref="Models.Quote"/> instance,
    /// or <c>null</c> if not found.
    /// </returns>
    public Task<Models.Quote> GetQuoteByIdAsync(string id, CancellationToken cancellationToken);
}
