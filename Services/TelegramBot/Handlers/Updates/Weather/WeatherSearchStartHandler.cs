using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Updates.Weather;

public class WeatherSearchStartHandler(
    ITelegramBotClient botClient
) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;

    public bool CanHandle(Update update)
        => update.Message?.Text == BotButtons.Texts.Weather.SearchCity;

    public async Task HandleAsync(Update update)
    {
        long chatId = update.Message!.Chat.Id;

        _ = await _botClient.SendMessage(
            chatId,
            "Send me the city name you want to check:"
        );
    }
}
