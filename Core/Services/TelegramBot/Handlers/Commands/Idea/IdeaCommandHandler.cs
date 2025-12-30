using Core.Services.Idea;
using Core.Services.TelegramBot.Interfaces;
using Core.Utils.UI;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Core.Services.TelegramBot.Handlers.Commands.Idea;

/// <summary>
/// Handles the <c>/idea</c> command for storing and retrieving user ideas.
/// </summary>
/// <remarks>
/// Supported subcommands:
/// <list type="bullet">
/// <item><description><c>create &lt;text&gt;</c> — Saves a new idea</description></item>
/// <item><description><c>list</c> — Lists saved ideas</description></item>
/// </list>
/// </remarks>
public sealed class IdeaCommandHandler(
    ITelegramBotClient botClient,
    IIdeaService ideaService,
    ILogger<IdeaCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IIdeaService _ideaService = ideaService;
    private readonly ILogger<IdeaCommandHandler> _logger = logger;

    /// <inheritdoc />
    public string Command => BotButtons.Commands.Ideas;

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Type is not UpdateType.Message || update.Message?.Text is null)
        {
            return;
        }

        await HandleIdeaCommand(update.Message.Chat.Id, update.Message.Text, cancellationToken);
    }

    private async Task HandleIdeaCommand(long chatId, string messageText, CancellationToken cancellationToken = default)
    {
        try
        {
            string[] parts = messageText.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length is < 2)
            {
                _ = await _botClient.SendMessage(
                    chatId,
                    "Use:\n/idea create <text>\n\nOr:\n/idea list 💡",
                    ParseMode.MarkdownV2,
                    cancellationToken: cancellationToken
                );
                return;
            }

            string subCommand = parts[1].ToLowerInvariant();
            string content = parts.Length is 3 ? parts[2] : string.Empty;

            switch (subCommand)
            {
                case "create":
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _ = await _botClient.SendMessage(chatId, "You need to write something ✍️", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                        return;
                    }

                    _ = await _ideaService.AddIdeaAsync(chatId, content, cancellationToken: cancellationToken);
                    _ = await _botClient.SendMessage(chatId, "💡 Idea saved!", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                    break;

                case "list":
                    List<Contracts.Models.Idea> ideas = await _ideaService
                        .GetUserIdeasAsync(chatId, cancellationToken)
                        .ToListAsync(cancellationToken: cancellationToken);

                    if (ideas.Count is 0)
                    {
                        _ = await _botClient.SendMessage(chatId, "You haven’t saved any ideas yet 🤔", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                        return;
                    }

                    string text = string.Join(
                        "\n\n",
                        ideas.Select(i => $"💭 {i.Label ?? "Idea"}: {i.Content}")
                    );

                    _ = await _botClient.SendMessage(chatId, text, ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                    break;

                default:
                    _ = await _botClient.SendMessage(chatId, "Unknown subcommand 😅", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling idea command");
            _ = await _botClient.SendMessage(chatId, "Something went wrong 😢 Try again later.", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        }
    }
}
