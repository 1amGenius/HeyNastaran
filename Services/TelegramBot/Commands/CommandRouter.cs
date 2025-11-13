using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nastaran_bot.Services.TelegramBot.Commands;

public class CommandRouter(IEnumerable<ICommandHandler> commandHandlers, ITelegramBotClient botClient)
{
    private readonly IEnumerable<ICommandHandler> _commandHandlers = commandHandlers;
    private readonly ITelegramBotClient _botClient = botClient;

    public async Task RouteAsync(Update update)
    {
        if (update.Message == null || string.IsNullOrWhiteSpace(update.Message.Text))
        {
            return;
        }

        long chatId = update.Message.Chat.Id;
        string messageText = update.Message.Text.Trim();

        string[] parts = messageText
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        string command = parts[0].ToLower();

        ICommandHandler handler = _commandHandlers
            .FirstOrDefault(h => h.Command
            .Equals(command, StringComparison.OrdinalIgnoreCase));

        if (handler != null)
        {
            await handler
                .HandleAsync(update);
        }
        else
        {
            _ = await _botClient
                .SendMessage(chatId, "Sorry, I don’t recognize that command 😅");
        }
    }
}
