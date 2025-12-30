using Core.Repositories.Idea;

namespace Core.Services.Idea;

/// <summary>
/// Provides high-level operations for creating, retrieving, and managing <see cref="Contracts.Models.Idea"/>.
/// Encapsulates business logic and cancellation-aware execution over the repository layer.
/// </summary>
public sealed class IdeaService(IIdeaRepository ideaRepository) : IIdeaService
{
    private readonly IIdeaRepository _ideaRepository = ideaRepository;

    /// <inheritdoc />
    public async Task<Contracts.Models.Idea> AddIdeaAsync(
        long telegramId,
        string content,
        string label = null,
        List<string> tags = null,
        bool favorite = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId, nameof(telegramId));
        ArgumentException.ThrowIfNullOrWhiteSpace(content, nameof(content));

        var idea = new Contracts.Models.Idea
        {
            TelegramId = telegramId,
            Content = content,
            Label = label ?? string.Empty,
            Tags = tags ?? [],
            Favorite = favorite,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _ideaRepository.AddAsync(idea, cancellationToken).ConfigureAwait(false);

        return idea;
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Contracts.Models.Idea> GetUserIdeasAsync(long telegramId, CancellationToken cancellationToken = default) 
        => telegramId is <= 0
            ? throw new ArgumentOutOfRangeException(nameof(telegramId), "TelegramId must be a positive number.")
            : _ideaRepository.GetByTelegramIdAsync(telegramId, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> DeleteIdeaAsync(string id, CancellationToken cancellationToken = default) 
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "Idea ID cannot be null or empty")
            : await _ideaRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Contracts.Models.Idea> GetIdeaByIdAsync(string id, CancellationToken cancellationToken = default) 
        => string.IsNullOrWhiteSpace(id)
            ? throw new ArgumentNullException(nameof(id), "Idea ID cannot be null or empty")
            : await _ideaRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
}
