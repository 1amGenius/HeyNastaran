using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Services.User;
using Nastaran_bot.Utils;

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
        long chatId = update.Message.Chat.Id;

        string username = update.Message.Chat.Username ?? string.Empty;
        string firstName = update.Message.Chat.FirstName ?? "Nastaran";

        try
        {
            Models.User existingUser = await _userService.GetUserByTelegramIdAsync(telegramId);

            if (existingUser != null)
            {
                _ = await _botClient.SendMessage(chatId, $"Welcome back, {firstName}! 🎉");
                return;
            }

            _ = await _userService.AddUserAsync(telegramId, username, firstName, "UTC");

            var keyboard = new ReplyKeyboardMarkup(new[]
            {
                new KeyboardButton[] { BotButtons.Texts.Songs },
                [BotButtons.Texts.Quotes], // daily, random, by category
                [BotButtons.Texts.Weather], // current, weekly, daily, hourly
                [BotButtons.Texts.Notes], //Daily, monthly, yearly
                [BotButtons.Texts.Ideas], // things to achieve, to buy, to read
                [BotButtons.Texts.Inspirations], // images, articles, everything that can be stored in mongo
                [BotButtons.Texts.Settings], //prefrences, language
                [BotButtons.Texts.Help] // how to use the bot
            })
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = false,
                IsPersistent = true,
                InputFieldPlaceholder = "Choose an option..."
            };

            string welcomeMessage =
$@"Hello {firstName}! 👋
I'm your personal bot.

You can get daily music, quotes, and weather updates right here.
Use the keyboard below to get started:";

            _ = await _botClient.SendMessage(chatId, welcomeMessage, replyMarkup: keyboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling /start for TelegramId {TelegramId}", telegramId);

            _ = await _botClient.SendMessage(chatId,
                "⚠️ Something went wrong while setting up your account.");
        }
    }
}
