using Core.Services.TelegramBot.Interfaces;
using Core.Services.User;
using Core.Utils.UI;

using Microsoft.Extensions.Logging;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Core.Services.TelegramBot.Handlers.Commands;

/// <summary>
/// Handles the <c>/start</c> command and initializes a new user session.
/// </summary>
/// <remarks>
/// Responsibilities:
/// <list type="bullet">
/// <item><description>
/// Registers new users in the system using their Telegram identity.
/// </description></item>
/// <item><description>
/// Welcomes returning users without duplicating data.
/// </description></item>
/// <item><description>
/// Presents the initial navigation keyboard to first-time users.
/// </description></item>
/// </list>
/// <para>
/// This handler is idempotent: invoking <c>/start</c> multiple times does not
/// create duplicate users.
/// </para>
/// </remarks>
public sealed class StartCommandHandler(
    ITelegramBotClient botClient,
    ILogger<StartCommandHandler> logger,
    IUserService userService
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger<StartCommandHandler> _logger = logger;
    private readonly IUserService _userService = userService;

    /// <inheritdoc />
    public string Command => BotButtons.Commands.Start;

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        if (update.Type is not UpdateType.Message || update.Message?.Text is null)
        {
            return;
        }

        long telegramId = update.Message.From.Id;
        long chatId = update.Message.Chat.Id;

        string username = update.Message.Chat.Username ?? string.Empty;
        string firstName = update.Message.Chat.FirstName ?? "Nastaran";

        try
        {
            Contracts.Models.User existingUser = await _userService.GetUserByTelegramIdAsync(telegramId, cancellationToken);

            if (existingUser is not null)
            {
                _ = await _botClient.SendMessage(chatId, $"Welcome back, {firstName}! 🎉", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
                return;
            }

            _ = await _userService.AddUserAsync(telegramId, username, firstName, "UTC", cancellationToken);

            string welcomeMessage =
$@"Hello {firstName}! 👋
I'm your personal bot.

You can get daily music, quotes, and weather updates right here.
Use the keyboard below to get started:";

            _ = await _botClient.SendMessage(chatId, welcomeMessage, replyMarkup: BotButtons.Keyboards.StartMenu, cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling /start for TelegramId {TelegramId}", telegramId);

            _ = await _botClient.SendMessage(chatId, "⚠️ Something went wrong while setting up your account.", ParseMode.MarkdownV2, cancellationToken: cancellationToken);
        }
    }
}
