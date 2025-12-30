using Core.Contracts.Models;
using Core.Services.TelegramBot.Interfaces;
using Core.Utils.Formaters;
using Core.Utils.Helpers.Weather.Interfaces;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Core.Services.TelegramBot.Handlers.Updates.Weather;

/// <summary>
/// Handles inline city-based weather requests issued via the <c>/weather &lt;city&gt;</c> command.
/// </summary>
/// <remarks>
/// This handler enables quick, stateless weather lookups without relying on
/// stored user location.
/// <para>
/// Example:
/// </para>
/// <code>
/// /weather London
/// </code>
/// <para>
/// It is selected only when a city name is provided alongside the command.
/// </para>
/// </remarks>
public sealed class WeatherCityHandler(
    ITelegramBotClient botClient,
    IWeatherApiClient weatherApi,
    ILogger<WeatherCityHandler> logger
) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IWeatherApiClient _weatherApi = weatherApi;
    private readonly ILogger<WeatherCityHandler> _logger = logger;

    /// <inheritdoc />
    public bool CanHandle(Update update)
    {
        if (update.Message?.Text is null)
        {
            return false;
        }

        string text = update.Message.Text.Trim().ToLowerInvariant();

        return text.StartsWith("/weather") && text.Split(' ').Length > 1;
    }

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        try
        {
            Message message = update.Message;
            if (message is null || string.IsNullOrWhiteSpace(message.Text))
            {
                return;
            }

            long chatId = message.Chat.Id;

            string[] parts = message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length is < 2)
            {
                _ = await _botClient.SendMessage(chatId, "Bruh you gotta type a city too 😭\nTry: `/weather London`", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                return;
            }

            string city = string.Join(' ', parts.Skip(1)).Trim();

            (float latitude, float longitude) = await _weatherApi.GetCoordinatesByCityNameAsync(city, cancellationToken);

            Contracts.Models.Weather weather = await _weatherApi.GetFullWeatherReportAsync(latitude, longitude, cancellationToken);
            CurrentWeather cw = weather.Current;

            _ = await _botClient.SendMessage(chatId, FormatWeather.Full(city, cw), ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling city weather");

            _ = await _botClient.SendMessage(
                update.Message?.Chat.Id ?? 0,
                "⚠️ Couldn't fetch weather for that city. It either doesn’t exist or the API had a meltdown 💀",
                cancellationToken: cancellationToken
            );
        }
    }
}
