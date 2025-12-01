using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Commands.Weather;

public class WeatherCommandHandler(
    ITelegramBotClient botClient,
    ILogger<WeatherCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger<WeatherCommandHandler> _logger = logger;

    public string Command => BotButtons.Commands.Weather;

    public async Task HandleAsync(Update update)
    {
        try
        {
            long chatId = update.Message!.Chat.Id;
            if (update.Message.Location == null)
            {
                _ = await _botClient.SendMessage(
                    chatId,
                    "To show weather at your location, please tap the button below:",
                    replyMarkup: BotButtons.Keyboards.RequestLocationMenu
                );
            }

            _ = await _botClient.SendMessage(
                chatId,
                "Choose a weather option:",
                replyMarkup: BotButtons.Keyboards.WeatherMenu
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling /weather command");
        }
    }
}
