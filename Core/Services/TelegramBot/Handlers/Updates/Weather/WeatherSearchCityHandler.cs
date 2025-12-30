using Core.Contracts.Dtos.User;
using Core.Services.TelegramBot.Interfaces;
using Core.Services.User;
using Core.Utils.Formaters;
using Core.Utils.Helpers.Weather.Interfaces;
using Core.Utils.Mappers;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Core.Services.TelegramBot.Handlers.Updates.Weather;

/// <summary>
/// Handles free-text city input during an active weather city search flow.
/// </summary>
/// <remarks>
/// This handler is state-driven and only activates when the user is marked
/// as actively searching for a city.
/// <para>
/// Responsibilities:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Resolves city names to coordinates.
/// </description></item>
/// <item><description>
/// Fetches and displays current weather for the specified city.
/// </description></item>
/// <item><description>
/// Gracefully handles invalid or unknown city names.
/// </description></item>
/// </list>
/// <para>
/// This handler relies on user state rather than explicit commands.
/// </para>
/// </remarks>
public sealed class WeatherSearchCityHandler(
    ITelegramBotClient botClient,
    IWeatherApiClient weatherApi,
    IUserService userService,
    ILogger<WeatherSearchCityHandler> logger
) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IWeatherApiClient _weatherApi = weatherApi;
    private readonly IUserService _userService = userService;
    private readonly ILogger<WeatherSearchCityHandler> _logger = logger;

    /// <inheritdoc />
    public bool CanHandle(Update update)
    {
        if (update.Message?.Text is null)
        {
            return false;
        }

        Contracts.Models.User user = _userService.GetUserByTelegramIdAsync(update.Message.From.Id).Result;
        return user is not null && user.IsSearchingCity && update.Message.Text.Length is >= 2;
    }

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        long chatId = update.Message!.Chat.Id;
        long telegramId = update.Message.From.Id;
        string city = update.Message.Text.Trim();

        try
        {
            (float lat, float lon) = await _weatherApi.GetCoordinatesByCityNameAsync(city, cancellationToken);
            Contracts.Models.Weather weather = await _weatherApi.GetCurrentWeatherAsync(lat, lon, cancellationToken);

            _ = await _botClient.SendMessage(
                chatId,
                FormatWeather.CitySummary(city, weather.Current),
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch weather for city '{City}'", city);

            _ = await _botClient.SendMessage(
                chatId,
                "⚠️ I couldn't find weather information for that city. Try something like: `London`",
                parseMode: ParseMode.MarkdownV2,
                cancellationToken: cancellationToken
            );
        }
        finally
        {
            Contracts.Models.User user = await _userService.GetUserByTelegramIdAsync(telegramId, cancellationToken);
            if (user is not null)
            {
                UserUpdateDto userDto = UserMapper.ToUpdateDto(user);
                userDto.IsSearchingCity = false;
                _ = await _userService.UpdateUserAsync(user.Id, userDto, cancellationToken);
            }
        }
    }
}
