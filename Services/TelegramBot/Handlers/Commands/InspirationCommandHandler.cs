using Nastaran_bot.Services.Inspiration;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Commands;

public class InspirationCommandHandler(
    ITelegramBotClient botClient,
    IInspirationService inspirationService,
    ILogger<InspirationCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IInspirationService _inspirationService = inspirationService;
    private readonly ILogger<InspirationCommandHandler> _logger = logger;

    public string Command => "/inspiration";

    public async Task HandleAsync(Update update)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
        {
            return;
        }

        long chatId = update.Message.Chat.Id;
        string messageText = update.Message.Text.Trim();

        if (messageText.StartsWith(Command, StringComparison.OrdinalIgnoreCase))
        {
            await HandleInspirationCommand(chatId, messageText);
        }
    }

    private async Task HandleInspirationCommand(long chatId, string messageText)
    {
        try
        {
            string[] parts = messageText.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                _ = await _botClient.SendMessage(
                    chatId,
                    "Use the format:\n`/inspiration create your inspiration text`\n\nYou can also try `/inspiration list` to view saved inspirations ✨",
                    parseMode: ParseMode.Markdown
                );
                return;
            }

            string subCommand = parts[1].ToLowerInvariant();

            switch (subCommand)
            {
                case "create":
                    _ = await _botClient.SendMessage(chatId,
                        "Use `/inspiration create <text>` to add your own inspirational quote 💫");
                    break;

                case "list":
                    IEnumerable<Models.Inspiration> inspirations = await _inspirationService.GetUserInspirationsAsync(chatId);
                    if (!inspirations.Any())
                    {
                        _ = await _botClient.SendMessage(chatId, "You have no saved inspirations yet 🌱");
                        return;
                    }

                    string list = string.Join("\n\n", inspirations.Select(i => $"🌻 {i.Content}"));
                    _ = await _botClient.SendMessage(chatId, list);
                    break;

                default:
                    _ = await _botClient.SendMessage(chatId,
                        "Unknown command 😅 Try `/inspiration` or `/inspiration list`");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling inspiration command");
            _ = await _botClient.SendMessage(chatId, "Something went wrong 😢 Try again later.");
        }
    }
}
