using Nastaran_bot.Services.Idea;
using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Utils;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Commands;

public class IdeaCommandHandler(
    ITelegramBotClient botClient,
    IIdeaService ideaService,
    ILogger<IdeaCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IIdeaService _ideaService = ideaService;
    private readonly ILogger<IdeaCommandHandler> _logger = logger;

    public string Command => BotButtons.Commands.Ideas;

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
            await HandleIdeaCommand(chatId, messageText);
        }
    }

    private async Task HandleIdeaCommand(long chatId, string messageText)
    {
        try
        {
            string[] parts = messageText.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                _ = await _botClient.SendMessage(
                    chatId,
                    "Use the format:\n`/idea create your idea text`\n\nYou can also try `/idea list` to view saved ideas 💡",
                    parseMode: ParseMode.MarkdownV2
                );
                return;
            }

            string subCommand = parts[1].ToLowerInvariant();
            string content = parts.Length == 3 ? parts[2] : string.Empty;

            switch (subCommand)
            {
                case "create":
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _ = await _botClient.SendMessage(chatId, "You need to write something for your idea ✍️");
                        return;
                    }

                    _ = await _ideaService.AddIdeaAsync(chatId, content);
                    _ = await _botClient.SendMessage(chatId, "Got it — your idea’s saved 💡");
                    break;

                case "list":
                    IEnumerable<Models.Idea> ideas = await _ideaService.GetUserIdeasAsync(chatId);

                    if (ideas.Any())
                    {
                        string text = string.Join("\n\n", ideas.Select(i => $"💭 *{i.Label ?? "idea"}*: {i.Content}"));
                        _ = await _botClient.SendMessage(chatId, text, parseMode: ParseMode.MarkdownV2);
                    }
                    else
                    {
                        _ = await _botClient.SendMessage(chatId, "You haven’t saved any ideas yet 🤔");
                    }

                    break;

                default:
                    _ = await _botClient.SendMessage(chatId, "Unknown subcommand 😅 Try `/idea create` or `/idea list`");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling idea command");
            _ = await _botClient.SendMessage(chatId, "Something went wrong 😢 Try again later.");
        }
    }
}
