using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Services.User;
using Nastaran_bot.Utils;
using Nastaran_bot.Utils.Formaters;
using Nastaran_bot.Utils.Helpers.Weather.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

using static Nastaran_bot.Utils.BotButtons;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Updates.Weather;

public class WeatherCallbackHandler(
    ITelegramBotClient botClient,
    IUserService userService,
    IWeatherApiClient weatherClient,
    ILogger<WeatherCallbackHandler> logger) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IUserService _userService = userService;
    private readonly IWeatherApiClient _weatherClient = weatherClient;
    private readonly ILogger<WeatherCallbackHandler> _logger = logger;

    public bool CanHandle(Update update)
    => update.Type == UpdateType.CallbackQuery &&
       update.CallbackQuery?.Data?.StartsWith("weather_") == true;

    public async Task HandleAsync(Update update)
    {
        if (update.CallbackQuery == null)
        {
            return;
        }

        CallbackQuery callback = update.CallbackQuery;
        long chatId = callback.Message.Chat.Id;
        string action = callback.Data;

        Models.User user = await _userService.GetUserByTelegramIdAsync(callback.From.Id);

        if (user.Location == null &&
            action != Actions.Weather.SearchCity)
        {
            _ = await _botClient.SendMessage(
                chatId,
                "Please share your location:",
                replyMarkup: BotButtons.Keyboards.RequestLocationMenu
            );

            return;
        }

        try
        {
            switch (action)
            {
                case var a when a == Actions.Weather.Current:
                    await SendCurrentWeather(chatId, user);
                    break;

                case var a when a == Actions.Weather.Hourly:
                    await SendHourlyWeather(chatId, user);
                    break;

                case var a when a == Actions.Weather.Daily:
                    await SendDailyWeather(chatId, user);
                    break;

                case var a when a == Actions.Weather.Weekly:
                    await SendWeeklyWeather(chatId, user);
                    break;

                case var a when a == Actions.Weather.SearchCity:
                    _ = await _botClient.SendMessage(chatId, "Send the city name:");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Weather callback handling error");
            _ = await _botClient.SendMessage(chatId, "⚠️ Failed to fetch weather data.");
        }
    }

    private async Task SendCurrentWeather(long chatId, Models.User user)
    {
        float lat = (float) user.Location.Lat;
        float lon = (float) user.Location.Lon;

        Models.Weather weather = await _weatherClient.GetCurrentWeatherAsync(lat, lon);
        string text = FormatWeather.Current(weather.Current);

        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2);
    }

    private async Task SendHourlyWeather(long chatId, Models.User user)
    {
        float lat = (float) user.Location.Lat;
        float lon = (float) user.Location.Lon;

        Models.Weather weather = await _weatherClient.GetHourlyForecastAsync(lat, lon);
        string text = FormatWeather.Hourly(weather.Hourly);

        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2);
    }

    private async Task SendDailyWeather(long chatId, Models.User user)
    {
        float lat = (float) user.Location.Lat;
        float lon = (float) user.Location.Lon;

        Models.Weather weather = await _weatherClient.GetDailyForecastAsync(lat, lon, 1);
        string text = FormatWeather.Daily(weather.Daily);

        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2);
    }

    private async Task SendWeeklyWeather(long chatId, Models.User user)
    {
        float lat = (float) user.Location.Lat;
        float lon = (float) user.Location.Lon;

        Models.Weather weather = await _weatherClient.GetDailyForecastAsync(lat, lon);
        string text = FormatWeather.Weekly(weather.Daily);

        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2);
    }
}
