using Core.Contracts.Common;
using Core.Repositories.Inspiration;

namespace Core.Services.Inspiration;

/// <summary>
/// Implements high-level inspiration management operations by coordinating This service
/// encapsulates all application-facing logic for creating, retrieving,
/// and deleting user inspiration items.
/// </summary>
public sealed class InspirationService(IInspirationRepository inspirationRepository) : IInspirationService
{
    private readonly IInspirationRepository _inspirationRepository = inspirationRepository;

    /// <inheritdoc />
    public async Task<Contracts.Models.Inspiration> AddInspirationAsync(
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

        var newInspiration = new Contracts.Models.Inspiration
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
    public IAsyncEnumerable<Contracts.Models.Inspiration> GetUserInspirationsAsync(long telegramId, CancellationToken cancellationToken = default) 
        => telegramId is <= 0
            ? throw new ArgumentOutOfRangeException(nameof(telegramId), "TelegramId must be a positive number.")
            : _inspirationRepository.GetByTelegramIdAsync(telegramId, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> DeleteInspirationAsync(string id, CancellationToken cancellationToken) 
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "Inspiration ID cannot be null or empty.")
            : await _inspirationRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Contracts.Models.Inspiration> GetInspirationByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        Contracts.Models.Inspiration inspiration = await _inspirationRepository
            .GetByIdAsync(id, cancellationToken)
            .ConfigureAwait(false);

        return inspiration is null ? throw new KeyNotFoundException("Inspiration not found.") : inspiration;
    }

    /// <inheritdoc />
    public async Task<PagedResult<Contracts.Models.Inspiration>> GetPageAsync(
        long userId,
        int page,
        int size,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ArgumentOutOfRangeException.ThrowIfNegative(page);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);

        int skip = page * size;

        (IReadOnlyList<Contracts.Models.Inspiration> items, int total) = await _inspirationRepository
            .GetPageAsync(userId, skip, size, cancellationToken)
            .ConfigureAwait(false);

        return new PagedResult<Contracts.Models.Inspiration>
        {
            Items = items,
            Page = page,
            PageSize = size,
            TotalCount = total
        };
    }

    /// <inheritdoc />
    public async Task UpdateContentAsync(string id, string content, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        await _inspirationRepository
            .UpdateContentAsync(id, content, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateTagsAsync(string id, List<string> tags, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        await _inspirationRepository
            .UpdateTagsAsync(id, tags, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateLabelAsync(string id, string label, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        await _inspirationRepository
            .UpdateLabelAsync(id, label, cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<bool> ToggleFavoriteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        return await _inspirationRepository
            .ToggleFavoriteAsync(id, cancellationToken)
            .ConfigureAwait(false);
    }
}
