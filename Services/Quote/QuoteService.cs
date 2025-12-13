using Nastaran_bot.Repositories.Quote;

namespace Nastaran_bot.Services.Quote;

/// <summary>
/// Implements high-level quote management operations by coordinating This service
/// encapsulates all application-facing logic for creating, retrieving,
/// and deleting user quote items.
/// </summary>
public class QuoteService(IQuoteRepository quoteRepository) : IQuoteService
{
    private readonly IQuoteRepository _quoteRepository = quoteRepository;

    /// <inheritdoc />
    public async Task<Models.Quote> AddQuoteAsync(
        long telegramId,
        string text,
        string category = "general",
        string author = "unknown",
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId);

        ArgumentException.ThrowIfNullOrWhiteSpace(text);

        var newNote = new Models.Quote
        {
            TelegramId = telegramId,
            Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            Text = text,
            Category = category,
            Author = author,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UsedAt = null,
        };

        await _quoteRepository.AddAsync(newNote, cancellationToken).ConfigureAwait(false);
        return newNote;
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.Quote> GetUserQuotesAsync(long telegramId, CancellationToken cancellationToken = default) 
        => telegramId <= 0
            ? throw new ArgumentOutOfRangeException(nameof(telegramId), "TelegramId must be a positive number.")
            : _quoteRepository.GetByTelegramIdAsync(telegramId, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> DeleteQuoteAsync(string id, CancellationToken cancellationToken) 
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "Quote ID cannot be null or empty.")
            : await _quoteRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Models.Quote> GetQuoteByIdAsync(string id, CancellationToken cancellationToken = default)
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "Quote ID cannot be null or empty.")
            : await _quoteRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
}
