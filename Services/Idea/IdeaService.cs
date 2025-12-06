using Nastaran_bot.Repositories.Idea;

namespace Nastaran_bot.Services.Idea;

/// <summary>
/// Provides high-level operations for creating, retrieving, and managing <see cref="Models.Idea"/>.
/// Encapsulates business logic, logging, and cancellation-aware execution over the repository layer.
/// </summary>
public class IdeaService(IIdeaRepository ideaRepository) : IIdeaService
{
    private readonly IIdeaRepository _ideaRepository = ideaRepository;

    /// <inheritdoc />
    public async Task<Models.Idea> AddIdeaAsync(
        long telegramId,
        string content,
        string label = null,
        List<string> tags = null,
        bool favorite = false,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(content);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId);

        var idea = new Models.Idea
        {
            TelegramId = telegramId,
            Content = content,
            Label = label ?? "idea",
            Tags = tags ?? [],
            Favorite = favorite,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        await _ideaRepository.AddAsync(idea, cancellationToken).ConfigureAwait(false);

        return idea;
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.Idea> GetUserIdeasAsync(long telegramId, CancellationToken cancellationToken = default) 
        => telegramId <= 0
            ? throw new ArgumentOutOfRangeException(nameof(telegramId), "TelegramId must be a positive number.")
            : _ideaRepository.GetByTelegramIdAsync(telegramId, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> DeleteIdeaAsync(string id, CancellationToken cancellationToken = default) 
        => string.IsNullOrWhiteSpace(id)
            ? throw new FormatException("Idea ID cannot be null or empty.")
            : await _ideaRepository.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

    /// <inheritdoc />
    public async Task<Models.Idea> GetIdeaByIdAsync(string id, CancellationToken cancellationToken = default) 
        => string.IsNullOrWhiteSpace(id)
            ? throw new FormatException("Idea ID cannot be null or empty.")
            : await _ideaRepository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
}
