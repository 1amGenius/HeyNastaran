using Core.Services.TelegramBot.Interfaces;
using Core.Utils.UI;
using Core.Utils.UI.Keyboards;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Core.Services.TelegramBot.Handlers.Commands.Weather;

/// <summary>
/// Handles the <c>/weather</c> command and initiates weather-related interactions.
/// </summary>
/// <remarks>
/// This handler acts as an entry point into the weather feature set.
/// <para>
/// Responsibilities:
/// </para>
/// <list type="bullet">
/// <item><description>
/// Requests the user's location when not already available.
/// </description></item>
/// <item><description>
/// Presents the weather options menu (current, hourly, daily, weekly, search).
/// </description></item>
/// </list>
/// <para>
/// Detailed weather retrieval is delegated to update handlers triggered
/// by callback queries or subsequent user input.
/// </para>
/// </remarks>
public sealed class WeatherCommandHandler(
    ITelegramBotClient botClient,
    ILogger<WeatherCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger<WeatherCommandHandler> _logger = logger;

    /// <inheritdoc />
    public string Command => BotButtons.Commands.Weather;

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        try
        {
            long chatId = update.Message!.Chat.Id;
            if (update.Message.Location is null)
            {
                _ = await _botClient.SendMessage(
                    chatId,
                    "To show weather at your location, please tap the button below:",
                    replyMarkup: WeatherKeyboards.RequestLocationMenu,
                    cancellationToken: cancellationToken
                );
            }

            _ = await _botClient.SendMessage(
                chatId,
                "Choose a weather option:",
                replyMarkup: WeatherKeyboards.WeatherMenu,
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling /weather command");
        }
    }
}
