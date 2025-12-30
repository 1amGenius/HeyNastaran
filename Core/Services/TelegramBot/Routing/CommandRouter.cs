using Core.Services.TelegramBot.Interfaces;

using Telegram.Bot.Types;

namespace Core.Services.TelegramBot.Routing;

/// <summary>
/// Routes command messages to their corresponding <see cref="ICommandHandler"/>.
/// </summary>
/// <remarks>
/// The router:
/// <list type="bullet">
/// <item><description>Extracts the command from message text.</description></item>
/// <item><description>Matches it against registered handlers.</description></item>
/// <item><description>Executes the first matching handler.</description></item>
/// </list>
/// If no handler matches, routing fails gracefully without throwing.
/// </remarks>
public sealed class CommandRouter(IEnumerable<ICommandHandler> commandHandlers)
{
    private readonly IEnumerable<ICommandHandler> _commandHandlers = commandHandlers;

    /// <summary>
    /// Attempts to route an update to a command handler.
    /// </summary>
    /// <param name="update">The update containing a command message.</param>
    /// <returns>
    /// <c>true</c> if a handler was found and executed; otherwise <c>false</c>.
    /// </returns>
    public async Task<bool> RouteAsync(Update update, CancellationToken cancellationToken = default)
    {
        string text = update.Message?.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false;
        }

        string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string command = parts[0].ToLowerInvariant();

        ICommandHandler handler = _commandHandlers
            .FirstOrDefault(h 
                => h.Command.Equals(command, StringComparison.OrdinalIgnoreCase));

        if (handler is null)
        {
            return false;
        }

        await handler.HandleAsync(update, cancellationToken);
        return true;
    }
}
