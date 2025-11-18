using Nastaran_bot.Services.TelegramBot.Interfaces;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Commands;

public class NoteCommandHandler(
    ITelegramBotClient botClient,
    ILogger<NoteCommandHandler> logger
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger<NoteCommandHandler> _logger = logger;

    public string Command => "/note";

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
                    "Use the format:\n`/note create your note text`\n\nYou can also try `/note list` to view your saved notes 📝",
                    parseMode: ParseMode.MarkdownV2
                );
                return;
            }

            string command = parts[1].ToLower();
            string content = parts.Length > 2 ? string.Join(' ', parts.Skip(2)) : "";

            switch (command)
            {
                case "create":
                    _ = await _botClient.SendMessage(
                        chatId,
                        $"📝 Note saved:\n\n{content}",
                        ParseMode.Html
                    );
                    break;

                case "list":
                    _ = await _botClient.SendMessage(
                        chatId,
                        "Here are your recent notes (coming soon) 📔"
                    );
                    break;

                default:
                    _ = await _botClient.SendMessage(chatId, "Unknown note command 😅");
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling note command");
            _ = await _botClient.SendMessage(update.Message.Chat.Id, "Something went wrong 😢");
        }
    }
}
