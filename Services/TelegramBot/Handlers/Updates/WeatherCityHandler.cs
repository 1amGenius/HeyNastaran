using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Utils.Helpers.Weather.Interfaces;

using OpenMeteo.Geocoding;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Updates;

public class WeatherCityHandler(
    ITelegramBotClient botClient,
    IWeatherApiClient weatherApi,
    ILogger<WeatherCityHandler> logger
) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IWeatherApiClient _weatherApi = weatherApi;
    private readonly ILogger<WeatherCityHandler> _logger = logger;

    public bool CanHandle(Update update)
    {
        if (update.Message?.Text == null)
        {
            return false;
        }

        string text = update.Message.Text.Trim().ToLower();

        return text.StartsWith("/weather") && text.Split(' ').Length > 1;
    }

    public async Task HandleAsync(Update update)
    {
        try
        {
            Message message = update.Message;
            if (message == null || string.IsNullOrWhiteSpace(message.Text))
            {
                return;
            }

            long chatId = message.Chat.Id;

            string[] parts = message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                _ = await _botClient.SendMessage(chatId, "Bruh you gotta type a city too 😭\nTry: `/weather London`", ParseMode.MarkdownV2);
                return;
            }

            string city = string.Join(' ', parts.Skip(1)).Trim();

            (float latitude, float longitude) = await _weatherApi.GetCoordinatesByCityNameAsync(city);
            
            Models.Weather weather = await _weatherApi.GetFullWeatherReportAsync(latitude, longitude);
            Models.CurrentWeather cw = weather.Current;

            string msg =
    $@"🌤 Weather in *{city}*

🌡 Temperature: {cw.TemperatureC:F1}°C
🥵 Feels like: {cw.FeelsLikeC:F1}°C
💧 Humidity: {cw.Humidity}%
🌬 Wind: {cw.WindSpeedKph:F1} km/h
☁️ Clouds: {cw.CloudCover}%
🌧 Rain chance: {cw.RainChance}%
🔆 UV Index: {cw.UvIndex:F1}

Condition: *{cw.Condition}* {cw.Icon}
";

            _ = await _botClient.SendMessage(chatId, msg, ParseMode.MarkdownV2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling city weather");

            _ = await _botClient.SendMessage(
                update.Message?.Chat.Id ?? 0,
                "⚠️ Couldn't fetch weather for that city. It either doesn’t exist or the API had a meltdown 💀"
            );
        }
    }
}
