using Core.Services.TelegramBot.Interfaces;
using Core.Services.User;
using Core.Utils.Formaters;
using Core.Utils.Helpers.Weather.Interfaces;
using Core.Utils.UI;
using Core.Utils.UI.Keyboards;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

using static Core.Utils.UI.BotButtons;

namespace Core.Services.TelegramBot.Handlers.Updates.Weather;

/// <summary>
/// Handles weather-related callback queries triggered by inline keyboard actions.
/// </summary>
/// <remarks>
/// This handler processes callback data prefixed with <c>weather_</c> and routes
/// the request to the appropriate weather operation.
/// <para>
/// Supported actions include:
/// </para>
/// <list type="bullet">
/// <item><description>Current weather</description></item>
/// <item><description>Hourly forecast</description></item>
/// <item><description>Daily forecast</description></item>
/// <item><description>Weekly forecast</description></item>
/// <item><description>City search initiation</description></item>
/// </list>
/// <para>
/// The handler depends on persisted user location data and prompts the user
/// to share their location when required.
/// </para>
/// </remarks>
public sealed class WeatherCallbackHandler(
    ITelegramBotClient botClient,
    IUserService userService,
    IWeatherApiClient weatherClient,
    ILogger<WeatherCallbackHandler> logger) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IUserService _userService = userService;
    private readonly IWeatherApiClient _weatherClient = weatherClient;
    private readonly ILogger<WeatherCallbackHandler> _logger = logger;

    /// <inheritdoc />
    public bool CanHandle(Update update)
    => update.Type == UpdateType.CallbackQuery &&
       update.CallbackQuery?.Data?.StartsWith("weather_") == true;

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.CallbackQuery is null)
        {
            return;
        }

        await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: cancellationToken);

        CallbackQuery callback = update.CallbackQuery;
        long chatId = callback.Message.Chat.Id;
        string action = callback.Data;

        Contracts.Models.User user = await _userService.GetUserByTelegramIdAsync(callback.From.Id, cancellationToken);

        if (user.Location is null &&
            action is not Actions.Weather.SearchCity)
        {
            _ = await _botClient.SendMessage(
                chatId,
                "Please share your location:",
                replyMarkup: WeatherKeyboards.RequestLocationMenu,
                cancellationToken: cancellationToken
            );

            return;
        }

        try
        {
            switch (action)
            {
                case var a when a is Actions.Weather.Current:
                    await SendCurrentWeather(chatId, user, cancellationToken);
                    break;

                case var a when a is Actions.Weather.Hourly:
                    await SendHourlyWeather(chatId, user, cancellationToken);
                    break;

                case var a when a is Actions.Weather.Daily:
                    await SendDailyWeather(chatId, user, cancellationToken);
                    break;

                case var a when a is Actions.Weather.Weekly:
                    await SendWeeklyWeather(chatId, user, cancellationToken);
                    break;

                case var a when a is Actions.Weather.SearchCity:
                    _ = await _botClient.SendMessage(chatId, "Send the city name:", cancellationToken: cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Weather callback handling error");
            _ = await _botClient.SendMessage(chatId, "⚠️ Failed to fetch weather data.", cancellationToken: cancellationToken);
        }
    }

    private async Task SendCurrentWeather(long chatId, Contracts.Models.User user, CancellationToken cancellationToken = default)
    {
        float lat = (float) user.Location.Lat;
        float lon = (float) user.Location.Lon;

        Contracts.Models.Weather weather = await _weatherClient.GetCurrentWeatherAsync(lat, lon, cancellationToken);
        string text = FormatWeather.Current(weather.Current);

        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
    }

    private async Task SendHourlyWeather(long chatId, Contracts.Models.User user, CancellationToken cancellationToken = default)
    {
        float lat = (float) user.Location.Lat;
        float lon = (float) user.Location.Lon;

        Contracts.Models.Weather weather = await _weatherClient.GetHourlyForecastAsync(lat, lon, cancellationToken: cancellationToken);
        string text = FormatWeather.Hourly(weather.Hourly);

        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
    }

    private async Task SendDailyWeather(long chatId, Contracts.Models.User user, CancellationToken cancellationToken = default)
    {
        float lat = (float) user.Location.Lat;
        float lon = (float) user.Location.Lon;

        Contracts.Models.Weather weather = await _weatherClient.GetDailyForecastAsync(lat, lon, 1, cancellationToken);
        string text = FormatWeather.Daily(weather.Daily);

        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
    }

    private async Task SendWeeklyWeather(long chatId, Contracts.Models.User user, CancellationToken cancellationToken = default)
    {
        float lat = (float) user.Location.Lat;
        float lon = (float) user.Location.Lon;

        Contracts.Models.Weather weather = await _weatherClient.GetDailyForecastAsync(lat, lon, cancellationToken: cancellationToken);
        string text = FormatWeather.Weekly(weather.Daily);

        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);
    }
}
