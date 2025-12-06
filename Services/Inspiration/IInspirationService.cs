namespace Nastaran_bot.Services.Inspiration;

/// <summary>
/// Provides high-level operations for creating, retrieving, and managing
/// user-submitted <see cref="Models.Inspiration"/> items such as mood-board images and captions.
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
    /// <returns>The created <see cref="Models.Inspiration"/> instance.</returns>
    public Task<Models.Inspiration> AddInspirationAsync(
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
    public IAsyncEnumerable<Models.Inspiration> GetUserInspirationsAsync(long telegramId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an inspiration entry by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the inspiration to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns><c>true</c> if the item was deleted; otherwise <c>false</c>.</returns>
    public Task<bool> DeleteInspirationAsync(string id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single inspiration item by its unique identifier.
    /// </summary>
    /// <param name="id">The identifier of the inspiration to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// The matching <see cref="Models.Inspiration"/> instance,
    /// or <c>null</c> if not found.
    /// </returns>
    public Task<Models.Inspiration> GetInspirationByIdAsync(string id, CancellationToken cancellationToken = default);
}
