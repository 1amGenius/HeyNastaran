using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Utils.Helpers.Weather;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Commands;

public class WeatherCommandHandler(
    ITelegramBotClient botClient,
    ILogger<WeatherCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger<WeatherCommandHandler> _logger = logger;

    public string Command => "/weather";

    public async Task HandleAsync(Update update)
    {
        try
        {
            string[] parts = update.Message!.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 1)
            {
                return; 
            }

            if (update.Type != UpdateType.Message || update.Message?.Text == null)
            {
                return;
            }

            long chatId = update.Message.Chat.Id;

            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[]
                {
                    KeyboardButton.WithRequestLocation("📍 Send your location")
                }
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
            };

            _ = await _botClient.SendMessage(
                chatId,
                "Share your live location to get accurate weather for where you are 🌦",
                replyMarkup: keyboard
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling /weather command");

            _ = await _botClient.SendMessage(
                update.Message!.Chat.Id,
                "⚠️ Something went wrong while requesting your location."
            );
        }
    }
}
