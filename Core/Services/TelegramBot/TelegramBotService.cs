using Core.Services.TelegramBot.Routing;
using Core.Utils.UI;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Core.Services.TelegramBot;

/// <summary>
/// Central entry point for processing incoming Telegram updates.
/// </summary>
/// <remarks>
/// This service orchestrates update handling by:
/// <list type="bullet">
/// <item><description>Distinguishing between commands, button presses, locations, and free-text updates.</description></item>
/// <item><description>Routing commands to <see cref="CommandRouter"/>.</description></item>
/// <item><description>Routing non-command updates to <see cref="UpdateRouter"/>.</description></item>
/// <item><description>Providing a fallback user message when no handler processes the update.</description></item>
/// </list>
/// This class should remain thin and free of business logic.
/// All behavior should be delegated to routers and handlers.
/// </remarks>
public sealed class TelegramBotService(
    ITelegramBotClient botClient,
    CommandRouter commandRouter,
    UpdateRouter updateRouter)
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly CommandRouter _commandRouter = commandRouter;
    private readonly UpdateRouter _updateRouter = updateRouter;

    /// <summary>
    /// Processes a single Telegram <see cref="Update"/>.
    /// </summary>
    /// <param name="update">
    /// The incoming update received from Telegram.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token to cancel operation.
    /// </param>
    /// <remarks>
    /// Routing rules:
    /// <list type="number">
    /// <item>
    /// <description>
    /// Callback queries triggered by inline keyboards are routed directly to
    /// <see cref="UpdateRouter"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Location messages are routed directly to <see cref="UpdateRouter"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Messages starting with <c>/</c> are treated as commands and routed to
    /// <see cref="CommandRouter"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Button texts mapped in <see cref="BotButtons.GlobalButtonsToCommand"/>
    /// are converted to synthetic commands.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// All other message-based updates are routed to <see cref="UpdateRouter"/>.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// If no command handler processes a command, a fallback help message
    /// is sent to the user.
    /// </para>
    /// </remarks>
    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken = default)

    {
        if (update is null)
        {
            return;
        }

        // 1. Inline keyboard clicks
        if (update.CallbackQuery is not null)
        {
            await _updateRouter.RouteAsync(update, cancellationToken);
            return;
        }

        // 2. Messages only from here
        if (update.Message is null)
        {
            return;
        }

        long chatId = update.Message.Chat.Id;
        string messageText = update.Message.Text;

        // 3. Location messages
        if (update.Message.Location is not null)
        {
            await _updateRouter.RouteAsync(update, cancellationToken);
            return;
        }

        bool handled;

        // 4. Slash commands
        if (!string.IsNullOrWhiteSpace(messageText) && messageText.StartsWith('/'))
        {
            handled = await _commandRouter.RouteAsync(update, cancellationToken);
        }
        // 5. Main menu buttons
        else if (!string.IsNullOrWhiteSpace(messageText) &&
                 BotButtons.GlobalButtonsToCommand.TryGetValue(messageText, out string command))
        {
            var syntheticUpdate = new Update
            {
                Id = update.Id,
                Message = new Message
                {
                    Chat = update.Message.Chat,
                    From = update.Message.From,
                    Text = command
                }
            };

            handled = await _commandRouter.RouteAsync(syntheticUpdate, cancellationToken);
        }
        // 6. Everything else
        else
        {
            await _updateRouter.RouteAsync(update, cancellationToken);
            handled = true;
        }

        if (!handled)
        {
            _ = await _botClient.SendMessage(
                chatId,
                "Send a command, click a button, or share your location 🌤",
                ParseMode.MarkdownV2,
                cancellationToken: cancellationToken);
        }
    }
}
