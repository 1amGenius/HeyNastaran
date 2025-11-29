using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Services.User;
using Nastaran_bot.Utils.Helpers.Weather.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Updates.Weather;

public class WeatherLocationHandler(
    ITelegramBotClient botClient,
    IWeatherApiClient weatherApi,
    ILogger<WeatherLocationHandler> logger,
    IUserService userService
) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IWeatherApiClient _weatherApi = weatherApi;
    private readonly ILogger<WeatherLocationHandler> _logger = logger;
    private readonly IUserService _userService = userService;

    public bool CanHandle(Update update)
        => update.Message?.Location != null;

    public async Task HandleAsync(Update update)
    {
        try
        {
            if (update.Message?.Location == null)
            {
                return;
            }

            long telegramId = update.Message.From.Id;
            long chatId = update.Message.Chat.Id;

            Models.User user = await _userService.GetUserByTelegramIdAsync(telegramId);
            if (user == null)
            {
                _ = await _botClient.SendMessage(chatId, "⚠️ Please use /start first.");
                return;
            }

            float lat = (float) update.Message.Location.Latitude;
            float lon = (float) update.Message.Location.Longitude;

            string id = _userService.GetUserByTelegramIdAsync(telegramId).Result!.Id;
            Contracts.Weather.ReverseGeocodingResult geo = await _weatherApi.GetCityAndCountryByCoordinatesAsync(lat, lon);

            user = await _userService.UpdateLocationAsync(id, new()
            {
                Lat = lat,
                Lon = lon,
                City = geo.City,
                Country = geo.Country
            });

            Models.Weather weather = await _weatherApi.GetFullWeatherReportAsync(lat, lon);
            Models.CurrentWeather cw = weather.Current;

            string msg =
$@"🌤 *Weather at {user.Location.City}*

🌡 Temperature: {cw.TemperatureC:F1}°C  
🥵 Feels like: {cw.FeelsLikeC:F1}°C  
💧 Humidity: {cw.Humidity}%  
🌬 Wind: {cw.WindSpeedKph:F1} km/h  
☁️ Clouds: {cw.CloudCover}%  
🌧 Rain chance: {cw.RainChance}%  
🔆 UV Index: {cw.UvIndex:F1}

Condition: *{cw.Condition}* {cw.Icon}
";

            _ = await _botClient.SendMessage(chatId, msg, parseMode: ParseMode.MarkdownV2);

            _ = await _botClient.SendMessage(chatId, "Done ✔", replyMarkup: new ReplyKeyboardRemove());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling location weather");

            _ = await _botClient.SendMessage(
                update.Message!.Chat.Id,
                "⚠️ Something went wrong while fetching weather."
            );
        }
    }
}
