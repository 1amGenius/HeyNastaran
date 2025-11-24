using Nastaran_bot.Services.TelegramBot.Routing;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nastaran_bot.Services.TelegramBot;

public class TelegramBotService(
    ITelegramBotClient botClient,
    CommandRouter commandRouter,
    UpdateRouter updateRouter
    )
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly CommandRouter _commandRouter = commandRouter;
    private readonly UpdateRouter _updateRouter = updateRouter;

    public async Task HandleUpdateAsync(Update update)
    {
        if (update?.Message == null)
        {
            return;
        }

        long chatId = update.Message.Chat.Id;
        string messageText = update.Message.Text;

        if (update.Message.Location != null)
        {
            await _updateRouter.RouteAsync(update);
        }
        else if (!string.IsNullOrWhiteSpace(messageText) && messageText.StartsWith("/"))
        {
            bool handled = await _commandRouter.RouteAsync(update);
            if (!handled)
            {
                _ = await _botClient.SendMessage(chatId, "Sorry, I don’t recognize that command 😅");
            }
        }
        else
        {
            _ = await _botClient.SendMessage(chatId, "Send a command or share your location 🌤");
        }
    }
}
