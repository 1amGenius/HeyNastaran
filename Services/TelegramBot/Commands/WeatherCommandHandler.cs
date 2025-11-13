using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Nastaran_bot.Services.TelegramBot.Commands;

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

            string fakeWeather = $"🌤️ The weather in {city} looks great today!";
            _ = await _botClient.SendMessage(chatId, fakeWeather);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling weather command");
            _ = await _botClient.SendMessage(update.Message.Chat.Id, "Something went wrong while fetching the weather 😢");
        }
    }
}
