using Nastaran_bot.Repositories.User;
using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Services.User;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Commands;

public class StartCommandHandler(
    ITelegramBotClient botClient,
    ILogger<StartCommandHandler> logger,
    IUserService userService
) : ICommandHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly ILogger<StartCommandHandler> _logger = logger;
    private readonly IUserService _userService = userService;

    public string Command => "/start";

    public async Task HandleAsync(Update update)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
        {
            return;
        }

        long telegramId = update.Message.From.Id;
        string username = update.Message.Chat.Username ?? string.Empty;
        string firstName = update.Message.Chat.FirstName ?? "Nastaran";

        try
        {
            IEnumerable<Models.User> existingUsers = await _userService.GetUsersAsync(telegramId);
            if (existingUsers.Any())
            {
                _ = await _botClient.SendMessage(update.Message.Chat.Id, $"Welcome back, {firstName}! 🎉");
                return;
            }

            Models.User newUser = await _userService.AddUserAsync(telegramId, username, firstName, "UTC");

            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { "🎵 Daily Music" },
                ["💬 Daily Quote"],
                ["🌤 Weather Updates"]
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false
            };

            string welcomeMessage =
$@"Hello {firstName}! 👋
I'm your personal bot.

You can get daily music, quotes, and weather updates right here.
Use the keyboard below to get started:";

            _ = await _botClient.SendMessage(update.Message.Chat.Id, welcomeMessage, replyMarkup: keyboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling /start command for chatId {ChatId}", update.Message.Chat.Id);
            _ = await _botClient.SendMessage(update.Message.Chat.Id, "⚠️ Something went wrong while setting up your account.");
        }
    }
}
