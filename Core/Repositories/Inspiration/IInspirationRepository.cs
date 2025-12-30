namespace Core.Repositories.Inspiration;

/// <summary>
/// Repository abstraction for Inspiration entities.
/// </summary>
public interface IInspirationRepository : IRepository<Contracts.Models.Inspiration>
{
    /// <summary>
    /// Retrieves a paginated list of inspiration items for a specific user.
    /// </summary>
    /// <param name="userId">
    /// The Telegram user identifier whose inspiration items are being requested.
    /// Must be greater than zero.
    /// </param>
    /// <param name="page">
    /// Zero-based page index.
    /// </param>
    /// <param name="size">
    /// Number of items per page. Must be greater than zero.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <returns>
    /// A <see cref="PagedResult{T}"/> containing the requested inspiration items
    /// along with pagination metadata.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="userId"/>, <paramref name="page"/>, or <paramref name="size"/> is out of range.
    /// </exception>
    public Task<(IReadOnlyList<Contracts.Models.Inspiration> Items, int TotalCount)> GetPageAsync(long userId, int skip, int take, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the textual content of an existing inspiration item.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the inspiration item.
    /// </param>
    /// <param name="content">
    /// The new content to be stored.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> is null or whitespace.
    /// </exception>
    public Task UpdateContentAsync(string id, string content, CancellationToken cancellationToken = default);

    /// <summary>
    /// Replaces the tag collection of an existing inspiration item.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the inspiration item.
    /// </param>
    /// <param name="tags">
    /// The complete list of tags to associate with the item.
    /// </param>
    /// <param name="cancellationToken">
    /// Token used to cancel the operation.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="id"/> is null or whitespace.
    /// </exception>
    public Task UpdateTagsAsync(string id, List<string> tags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the label of an existing inspiration item.
    /// </summary>
    /// <param name="id">
    /// The unique identifier of the inspiration item.
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
