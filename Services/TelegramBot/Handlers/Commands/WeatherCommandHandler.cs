using Nastaran_bot.Utils.Helpers.Weather;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Commands;

public class WeatherCommandHandler(
    ITelegramBotClient botClient,
    IWeatherApiClient weatherApi,
    ILogger<WeatherCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IWeatherApiClient _weatherApi = weatherApi;
    private readonly ILogger<WeatherCommandHandler> _logger = logger;

    public string Command => "/weather";

    public async Task HandleAsync(Update update)
    {
        try
        {
            update.Message.Location
            if (update.Type != UpdateType.Message || update.Message?.Text == null)
            {
                return;
            }

            long chatId = update.Message.Chat.Id;
            string[] parts = update.Message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                _ = await _botClient.SendMessage(
                    chatId,
                    "Use the format:\n`/weather city_name`\n\nExample: `/weather London` ☁️",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            string city = string.Join(' ', parts.Skip(1));

            // Resolve location => YOU MUST HAVE A GEO LOOKUP SERVICE
            // For now we mock it until your geocoder exists
            (double lat, double lon) = await ResolveCityCoordinates(city);

            Models.Weather weather = await _weatherApi.GetFullWeatherReportAsync(lat, lon);

            Models.CurrentWeather w = weather.Current;

            string msg =
$@"🌤 Weather in *{city}*

🌡 Temperature: {w.TemperatureC:F1}°C
🥵 Feels like: {w.FeelsLikeC:F1}°C
💧 Humidity: {w.Humidity}%
🌬 Wind: {w.WindSpeedKph:F1} km/h
☁️ Clouds: {w.CloudCover}%
🌧 Rain chance: {w.RainChance}%
🔆 UV Index: {w.UvIndex:F1}

Condition: *{w.Condition}* {w.Icon}
";

            _ = await _botClient.SendMessage(chatId, msg, parseMode: ParseMode.Markdown);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling /weather command");

            _ = await _botClient.SendMessage(
                update.Message.Chat.Id,
                "⚠️ Something went wrong while fetching weather."
            );
        }
    }
}
