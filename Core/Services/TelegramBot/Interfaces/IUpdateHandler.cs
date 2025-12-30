using Telegram.Bot.Types;

namespace Core.Services.TelegramBot.Interfaces;

/// <summary>
/// Defines a handler for non-command Telegram updates.
/// </summary>
/// <remarks>
/// Update handlers process contextual or state-driven interactions such as:
/// <list type="bullet">
/// <item><description>Callback queries</description></item>
/// <item><description>Location sharing</description></item>
/// <item><description>Free-text input following a previous action</description></item>
/// </list>
/// <para>
/// Unlike command handlers, update handlers:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Are selected dynamically using <see cref="CanHandle(Update)"/>.
/// </description></item>
/// <item><description>
/// May depend on user state, message content, or update type.
/// </description></item>
/// <item><description>
/// Are evaluated in registration order; the first match wins.
/// </description></item>
/// </list>
/// <para>
/// Update handlers are routed via <see cref="UpdateRouter"/>.
/// </para>
/// </remarks>
public interface IUpdateHandler
{
    /// <summary>
    /// Determines whether this handler can process the specified update.
    /// </summary>
    /// <param name="update">
    /// The incoming Telegram update.
    /// </param>
    /// <returns>
    /// <c>true</c> if this handler should handle the update; otherwise <c>false</c>.
    /// </returns>
    /// <remarks>
    /// This method must be:
    /// <list type="bullet">
    /// <item><description>Pure (no side effects)</description></item>
    /// <item><description>Fast (called frequently)</description></item>
    /// <item><description>Deterministic</description></item>
    /// </list>
    /// <para>
    /// No asynchronous work should be performed here.
    /// </para>
    /// </remarks>
    public bool CanHandle(Update update);

    /// <summary>
    /// Handles the specified update.
    /// </summary>
    /// <param name="update">
    /// The Telegram update to process.
    /// </param>
    /// <remarks>
    /// Implementations:
    /// <list type="bullet">
    /// <item><description>
    /// Can assume <see cref="CanHandle(Update)"/> has already returned <c>true</c>.
    /// </description></item>
    /// <item><description>
    /// Must handle invalid or partial updates defensively.
    /// </description></item>
    /// <item><description>
    /// Are responsible for all user-facing responses.
    /// </description></item>
    /// <item><description>
    /// Should catch and log exceptions internally.
    /// </description></item>
    /// </list>
    /// </remarks>
    public Task HandleAsync(Update update, CancellationToken cancellationToken = default);
}
