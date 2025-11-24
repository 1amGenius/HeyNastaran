using Nastaran_bot.Services.TelegramBot.Interfaces;

using Telegram.Bot.Types;

namespace Nastaran_bot.Services.TelegramBot.Routing;

public class CommandRouter(IEnumerable<ICommandHandler> commandHandlers)
{
    private readonly IEnumerable<ICommandHandler> _commandHandlers = commandHandlers;

    public async Task<bool> RouteAsync(Update update)
    {
        string text = update.Message?.Text;
        if (string.IsNullOrWhiteSpace(text))
        {
            return false; // not a command
        }

        string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string command = parts[0].ToLower();

        ICommandHandler handler = _commandHandlers
            .FirstOrDefault(h => h.Command.Equals(command, StringComparison.OrdinalIgnoreCase));

        if (handler == null)
        {
            return false;
        }

        await handler.HandleAsync(update);
        return true;
    }
}
