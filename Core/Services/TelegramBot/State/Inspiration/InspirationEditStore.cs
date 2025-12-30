using System.Collections.Concurrent;

namespace Core.Services.TelegramBot.State.Inspiration;

/// <summary>
/// In-memory store that tracks active inspiration edit sessions per user.
/// 
/// This allows callback-based edit selection (content/tags/label)
/// to be completed later via a plain text message.
/// </summary>
public sealed class InspirationEditStore
{
    /// <summary>
    /// Stores edit contexts keyed by Telegram user ID.
    /// </summary>
    private readonly ConcurrentDictionary<long, InspirationEditContext> _store = new();

    /// <summary>
    /// Sets or replaces the current edit context for a user.
    /// Called when the user selects an edit action from an inline keyboard.
    /// </summary>
    /// <param name="telegramId">Telegram user identifier.</param>
    /// <param name="context">Edit context describing what will be edited.</param>
    public void Set(long telegramId, InspirationEditContext context)
        => _store[telegramId] = context;

    /// <summary>
    /// Attempts to retrieve the current edit context for a user.
    /// </summary>
    /// <param name="telegramId">Telegram user identifier.</param>
    /// <param name="context">
    /// When this method returns, contains the edit context if found.
    /// </param>
    /// <returns>
    /// <c>true</c> if an edit context exists for the user; otherwise <c>false</c>.
    /// </returns>
    public bool TryGet(long telegramId, out InspirationEditContext context)
        => _store.TryGetValue(telegramId, out context);

    /// <summary>
    /// Clears the active edit context for a user.
    /// Called after a successful update to ensure edits are single-step
    /// and do not leak into subsequent messages.
    /// </summary>
    /// <param name="telegramId">Telegram user identifier.</param>
    public void Clear(long telegramId)
        => _store.TryRemove(telegramId, out _);
}
