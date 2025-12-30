using System.Collections.Concurrent;

namespace Core.Services.TelegramBot.State.Inspiration;

/// <summary>
/// Tracks a short-lived user intent to create a new inspiration.
/// 
/// This is used as a gate between a callback action (e.g. pressing "Add")
/// and the next incoming message (photo + caption).
/// 
/// The intent is single-use and is consumed atomically to prevent
/// duplicate or accidental creations.
/// </summary>
public sealed class InspirationCreateIntentStore
{
    /// <summary>
    /// In-memory store keyed by Telegram user ID.
    /// 
    /// Value is irrelevant; presence of the key indicates an active intent.
    /// </summary>
    private readonly ConcurrentDictionary<long, bool> _store = new();

    /// <summary>
    /// Enables creation intent for the specified user.
    /// 
    /// Typically called immediately after the user presses the
    /// "Add inspiration" callback button.
    /// </summary>
    /// <param name="telegramId">Telegram user identifier.</param>
    public void Enable(long telegramId)
        => _store[telegramId] = true;

    /// <summary>
    /// Consumes and clears the creation intent for the specified user.
    /// 
    /// Returns <c>true</c> if an intent existed and was removed;
    /// otherwise <c>false</c>.
    /// </summary>
    /// <param name="telegramId">Telegram user identifier.</param>
    public bool Consume(long telegramId)
        => _store.TryRemove(telegramId, out _);
}
