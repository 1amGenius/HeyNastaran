using Core.Contracts.Common;

namespace Core.Services.Inspiration;

/// <summary>
/// Provides high-level operations for creating, retrieving, and managing
/// user-submitted <see cref="Contracts.Models.Inspiration"/> items such as mood-board images and captions.
/// </summary>
public interface IInspirationService
{
    /// <summary>
    /// Creates a new inspiration entry for the specified Telegram user.
    /// </summary>
    /// <param name="telegramId">Identifier of the user creating the inspiration.</param>
    /// <param name="caption">Caption or descriptive text for the inspiration.</param>
    /// <param name="imageFileId">Identifier of the associated image file.</param>
    /// <param name="label">Optional category/label for organization.</param>
    /// <param name="tags">Optional list of tags for filtering and grouping.</param>
    /// <param name="favorite">Indicates whether the item is marked as a favorite.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created <see cref="Contracts.Models.Inspiration"/> instance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if Telegram ID is not a valid identifier (e.g., non-positive).
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if caption or imageFileId is null, whitespace or empty.
    /// </exception>
    public Task<Contracts.Models.Inspiration> AddInspirationAsync(
        long telegramId,
        string caption,
        string imageFileId,
        string label = "mood_board",
        List<string> tags = null,
        bool favorite = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all inspirations created by the specified Telegram user.
    /// </summary>
    /// <param name="telegramId">Identifier of the user whose inspirations will be returned.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A sequence of inspirations for the user.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if Telegram ID is not a valid identifier (e.g., non-positive).
    /// </exception>
    public IAsyncEnumerable<Contracts.Models.Inspiration> GetUserInspirationsAsync(long telegramId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an inspiration entry by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the inspiration to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the item was deleted; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided inspiration ID is null, empty, or whitespace.
    /// </exception>
    public Task<bool> DeleteInspirationAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single inspiration item by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the inspiration to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The matching <see cref="Contracts.Models.Inspiration"/> instance,
    /// or <c>null</c> if not found.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the provided inspiration ID is null, empty, or whitespace.
    /// </exception>
    public Task<Contracts.Models.Inspiration> GetInspirationByIdAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paginated subset of inspiration items belonging to a specific user.
    /// </summary>
    /// <param name="userId">
    /// The Telegram user identifier used to scope the query.
    /// Must be greater than zero.
    /// </param>
    /// <param name="page">
    /// Zero-based page index used to calculate the data offset.
    /// </param>
    /// <param name="size">
    /// Number of items to include per page.
    /// Must be greater than zero.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="PagedResult{T}"/> containing the requested page of inspiration items
    /// and pagination metadata.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="userId"/>, <paramref name="page"/>, or <paramref name="size"/> is invalid.
    /// </exception>
    public Task<PagedResult<Contracts.Models.Inspiration>> GetPageAsync(long userId, int page, int size, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the main textual content of an existing inspiration item.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the inspiration item to update.
    /// </param>
    /// <param name="content">
    /// The new content value to persist.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> is null or whitespace.
    /// </exception>
    public Task UpdateContentAsync(string id, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the tag collection associated with an inspiration item.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the inspiration item to update.
    /// </param>
    /// <param name="tags">
    /// The full set of tags to assign to the item.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> is null or whitespace.
    /// </exception>
    public Task UpdateTagsAsync(string id, List<string> tags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the human-readable label of an inspiration item.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the inspiration item to update.
    /// </param>
    /// <param name="label">
    /// The new label value.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> is null or whitespace.
    /// </exception>
    public Task UpdateLabelAsync(string id, string label, CancellationToken cancellationToken = default);

    /// <summary>
    /// Toggles the favorite state of an inspiration item.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the inspiration item.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// <c>true</c> if the item is marked as favorite after the operation;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> is null or whitespace.
    /// </exception>
    public Task<bool> ToggleFavoriteAsync(string id, CancellationToken cancellationToken = default);
}
