using Core.Contracts.Dtos.Weather;
using Core.Contracts.Models;
using Core.Services.TelegramBot.Interfaces;
using Core.Services.User;
using Core.Utils.Formaters;
using Core.Utils.Helpers.Weather.Interfaces;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Core.Services.TelegramBot.Handlers.Updates.Weather;

/// <summary>
/// Handles weather requests triggered by user-shared geographic locations.
/// </summary>
/// <remarks>
/// Responsibilities:
/// <list type="bullet">
/// <item><description>
/// Persists the user's geographic location.
/// </description></item>
/// <item><description>
/// Performs reverse geocoding to determine city and country.
/// </description></item>
/// <item><description>
/// Returns a full weather report for the shared location.
/// </description></item>
/// </list>
/// <para>
/// This handler is typically invoked after a location request initiated
/// by the weather command flow.
/// </para>
/// </remarks>
public sealed class WeatherLocationHandler(
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

    /// <inheritdoc />
    public bool CanHandle(Update update)
        => update.Message?.Location is not null;

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        try
        {
            if (update.Message?.Location is null)
            {
                return;
            }

            long telegramId = update.Message.From.Id;
            long chatId = update.Message.Chat.Id;

            Contracts.Models.User user = await _userService.GetUserByTelegramIdAsync(telegramId, cancellationToken);
            if (user is null)
            {
                _ = await _botClient.SendMessage(chatId, "⚠️ Please use `/start` first.", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                return;
            }

            float lat = (float) update.Message.Location.Latitude;
            float lon = (float) update.Message.Location.Longitude;

            string id = user.Id;
            ReverseGeocodingResult geo = await _weatherApi.GetCityAndCountryByCoordinatesAsync(lat, lon, cancellationToken);

            user = await _userService.UpdateLocationAsync(id, new()
            {
                Lat = lat,
                Lon = lon,
                City = geo.City,
                Country = geo.Country
            }, cancellationToken);

            Contracts.Models.Weather weather = await _weatherApi.GetFullWeatherReportAsync(lat, lon, cancellationToken);
            CurrentWeather cw = weather.Current;

 

            _ = await _botClient.SendMessage(chatId, FormatWeather.Full(user.Location.City, cw), parseMode: ParseMode.MarkdownV2, cancellationToken: cancellationToken);

            _ = await _botClient.SendMessage(chatId, "Done ✔", ParseMode.MarkdownV2, replyMarkup: new ReplyKeyboardRemove(), cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling location weather");

            _ = await _botClient.SendMessage(
                update.Message!.Chat.Id,
                "⚠️ Something went wrong while fetching weather.",
                cancellationToken: cancellationToken
            );
        }
    }
}
