using Nastaran_bot.Repositories.Inspiration;

namespace Nastaran_bot.Services.Inspiration;

/// <summary>
/// Implements high-level inspiration management operations by coordinating This service
/// encapsulates all application-facing logic for creating, retrieving,
/// and deleting user inspiration items.
/// </summary>
public class InspirationService(IInspirationRepository inspirationRepository) : IInspirationService
{
    private readonly IInspirationRepository _inspirationRepository = inspirationRepository;

    /// <inheritdoc />
    public async Task<Models.Inspiration> AddInspirationAsync(
        long telegramId,
        string caption,
        string imageFileId,
        string label = null,
        List<string> tags = null,
        bool favorite = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId, nameof(telegramId));
        ArgumentException.ThrowIfNullOrWhiteSpace(caption, nameof(caption));
        ArgumentException.ThrowIfNullOrWhiteSpace(imageFileId, nameof(imageFileId));

        var newInspiration = new Models.Inspiration
        {
            TelegramId = telegramId,
            Content = caption,
            Favorite = favorite,
            ImageFileId = imageFileId,
            Label = label ?? string.Empty,
            Tags = tags ?? [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _inspirationRepository.AddAsync(newInspiration, cancellationToken).ConfigureAwait(false);

        return newInspiration;
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.Inspiration> GetUserInspirationsAsync(long telegramId, CancellationToken cancellationToken = default) 
        => telegramId is <= 0
            ? throw new ArgumentOutOfRangeException(nameof(telegramId), "TelegramId must be a positive number.")
            : _inspirationRepository.GetByTelegramIdAsync(telegramId, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> DeleteInspirationAsync(string id, CancellationToken cancellationToken) 
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "Inspiration ID cannot be null or empty.")
            : await _inspirationRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Models.Inspiration> GetInspirationByIdAsync(string id, CancellationToken cancellationToken = default) 
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "Inspiration ID cannot be null or empty.")
            : await _inspirationRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
}
