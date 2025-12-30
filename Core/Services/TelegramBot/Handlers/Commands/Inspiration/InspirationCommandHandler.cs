using Core.Services.TelegramBot.Interfaces;
using Core.Utils.UI;
using Core.Utils.UI.Keyboards;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Core.Services.TelegramBot.Handlers.Commands.Inspiration;

/// <summary>
/// Handles the <c>/inspiration</c> command for managing inspirational quotes.
/// </summary>
/// <remarks>
/// Supported subcommands:
/// <list type="bullet">
/// <item><description><c>create &lt;text&gt;</c> — Saves a new inspiration</description></item>
/// <item><description><c>list</c> — Lists saved inspirations</description></item>
/// </list>
/// </remarks>
public sealed class InspirationCommandHandler(ITelegramBotClient botClient) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;

    public string Command => BotButtons.Commands.Inspirations;

    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        long chatId = update.Message.Chat.Id;

        _ = await _botClient.SendMessage(
            chatId,
            "🎀 Inspirations\n\nSave images + captions for later inspiration.",
            replyMarkup: InspirationKeyboards.InspirationsMenu,
            cancellationToken: cancellationToken
        );
    }
}
