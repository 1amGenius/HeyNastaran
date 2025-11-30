using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Utils.Formaters;
using Nastaran_bot.Utils.Helpers.Weather.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Updates.Weather;

public class WeatherSearchCityHandler(
    ITelegramBotClient botClient,
    IWeatherApiClient weatherApi,
    ILogger<WeatherSearchCityHandler> logger
) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IWeatherApiClient _weatherApi = weatherApi;
    private readonly ILogger<WeatherSearchCityHandler> _logger = logger;

    public bool CanHandle(Update update)
    {
        if (update.Message?.Text == null)
        {
            return false;
        }

        string text = update.Message.Text.Trim();

        return !text.StartsWith(char.Parse("/")) && text.Length >= 2;
    }

    public async Task HandleAsync(Update update)
    {
        long chatId = update.Message!.Chat.Id;
        string city = update.Message!.Text.Trim();

        try
        {
            (float lat, float lon) = await _weatherApi.GetCoordinatesByCityNameAsync(city);
            Models.Weather weather = await _weatherApi.GetCurrentWeatherAsync(lat, lon);

            _ = await _botClient.SendMessage(chatId,
                FormatWeather.CitySummary(city, weather.Current),
                parseMode: ParseMode.MarkdownV2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch weather for city '{City}'", city);

            _ = await _botClient.SendMessage(chatId,
                "⚠️ I couldn't find weather information for that city. Try something like: `London`",
                parseMode: ParseMode.MarkdownV2);
        }
    }
}
