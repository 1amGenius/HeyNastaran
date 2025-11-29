using Nastaran_bot.Services.TelegramBot.Routing;
using Nastaran_bot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
            return;
        }

        bool handled;

        if (!string.IsNullOrWhiteSpace(messageText) && messageText.StartsWith(char.Parse("/")))
        {
            handled = await _commandRouter.RouteAsync(update);
        }
        else if (!string.IsNullOrWhiteSpace(messageText) &&
                 BotButtons.GlobalButtonsToCommand.TryGetValue(messageText, out string command))
        {
            var syntheticUpdate = new Update
            {
                Id = update.Id,
                Message = new Message
                {
                    Chat = update.Message.Chat,
                    From = update.Message.From,
                    Text = command
                }
            };

            handled = await _commandRouter.RouteAsync(syntheticUpdate);
        }
        else
        {
            await _updateRouter.RouteAsync(update);
            handled = true;
        }

        if (!handled)
        {
            _ = await _botClient.SendMessage(chatId, "Send a command, click a button, or share your location 🌤");
        }
    }
}
