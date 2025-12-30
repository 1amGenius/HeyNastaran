using Core.Services.TelegramBot.Interfaces;
using Core.Utils.UI;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Core.Services.TelegramBot.Handlers.Commands.Note;

/// <summary>
/// Handles the <c>/note</c> command and manages simple user notes.
/// </summary>
/// <remarks>
/// Supported subcommands:
/// <list type="bullet">
/// <item><description><c>create &lt;text&gt;</c> — Saves a new note</description></item>
/// <item><description><c>list</c> — Displays saved notes (placeholder)</description></item>
/// </list>
///
/// <para>
/// This handler currently demonstrates command parsing and user interaction;
/// persistent note storage can be introduced without changing the command contract.
/// </para>
/// </remarks>
public sealed class NoteCommandHandler(
    ITelegramBotClient botClient,
    ILogger<NoteCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger<NoteCommandHandler> _logger = logger;

    /// <inheritdoc />
    public string Command => BotButtons.Commands.Notes;

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        try
        {
            if (update.Type is not UpdateType.Message || update.Message?.Text is null)
            {
                return;
            }

            long chatId = update.Message.Chat.Id;
            string[] parts = update.Message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length is < 2)
            {
                _ = await _botClient.SendMessage(
                    chatId,
                    "Use:\n/note create <text>\n\nOr:\n/note list 📝",
                    ParseMode.MarkdownV2,
                    cancellationToken: cancellationToken
                );
                return;
            }

            string subCommand = parts[1].ToLowerInvariant();
            string content = parts.Length is > 2 ? string.Join(' ', parts.Skip(2)) : string.Empty;

            switch (subCommand)
            {
                case "create":
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        _ = await _botClient.SendMessage(chatId, "You need to write something for the note ✍️", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                        return;
                    }

                    _ = await _botClient.SendMessage(chatId, $"📝 Note saved:\n\n{content}", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                    break;

                case "list":
                    _ = await _botClient.SendMessage(chatId, "Here are your recent notes (coming soon) 📔", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                    break;

                default:
                    _ = await _botClient.SendMessage(chatId, "Unknown note command 😅", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling note command");
            _ = await _botClient.SendMessage(update.Message!.Chat.Id, "Something went wrong 😢", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        }
    }
}
