using Telegram.Bot.Types;

namespace Core.Services.TelegramBot.Interfaces;

/// <summary>
/// Defines a handler responsible for executing a single Telegram bot command.
/// </summary>
/// <remarks>
/// A command handler represents a high-level, explicit user intent,
/// triggered by a message starting with a slash (e.g. <c>/start</c>, <c>/weather</c>)
/// or by a button mapped to a command.
/// <para>
/// Design rules:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Each implementation must handle exactly one command.
/// </description></item>
/// <item><description>
/// Command matching is case-insensitive and based on the first token of the message.
/// </description></item>
/// <item><description>
/// Handlers should assume routing guarantees that the command matches <see cref="Command"/>.
/// </description></item>
/// <item><description>
/// Handlers should be side-effect safe and tolerate malformed updates defensively.
/// </description></item>
/// </list>
/// <para>
/// Command handlers are routed via <see cref="CommandRouter"/>.
/// </para>
/// </remarks>
public interface ICommandHandler
{
    /// <summary>
    /// Gets the command string this handler responds to.
    /// </summary>
    /// <remarks>
    /// This value is compared against the first token of the incoming message text.
    /// It should include the leading slash.
    ///
    /// <para>Examples:</para>
    /// <list type="bullet">
    /// <item><description><c>/start</c></description></item>
    /// <item><description><c>/weather</c></description></item>
    /// </list>
    /// </remarks>
    public string Command
    {
        get;
    }

    /// <summary>
    /// Executes the command logic for the given Telegram update.
    /// </summary>
    /// <param name="update">
    /// The Telegram update containing the command message.
    /// </param>
    /// <remarks>
    /// Implementations:
    /// <list type="bullet">
    /// <item><description>
    /// Must not assume non-null message content unless validated.
    /// </description></item>
    /// <item><description>
    /// Should handle errors internally and avoid throwing.
    /// </description></item>
    /// <item><description>
    /// Are responsible for sending all user-facing responses.
    /// </description></item>
    /// </list>
    /// </remarks>
    public Task HandleAsync(Update update, CancellationToken cancellationToken = default);
}
