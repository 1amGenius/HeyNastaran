using Nastaran_bot.Services.DailyNote;
using Nastaran_bot.Services.Idea;
using Nastaran_bot.Services.Inspiration;
using Nastaran_bot.Services.TelegramBot.Handlers.Commands;
using Nastaran_bot.Services.User;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Nastaran_bot.Services.TelegramBot;

public class TelegramBotService(
    IUserService userService,
    IDailyNoteService dailyNoteService,
    IIdeaService ideaService,
    IInspirationService inspirationService,
    ITelegramBotClient botClient,
    CommandRouter commandRouter)
{
    private readonly IUserService _userService = userService;
    private readonly IDailyNoteService _dailyNoteService = dailyNoteService;
    private readonly IIdeaService _ideaService = ideaService;
    private readonly IInspirationService _inspirationService = inspirationService;

    private readonly ITelegramBotClient _botClient = botClient;
    private readonly CommandRouter _commandRouter = commandRouter;

    public async Task HandleUpdateAsync(Update update)
    {
        if (update?.Message == null)
        {
            return;
        }

        long chatId = update.Message.Chat.Id;
        string messageText = update.Message.Text;

        if (!string.IsNullOrWhiteSpace(messageText) && messageText.StartsWith(char.Parse("/")))
        {
            await _commandRouter.RouteAsync(update);
        }
        else
        {
            _ = await _botClient.SendMessage(
                chatId,
                "Try sending a command like <code>/note</code> 🌤",
                parseMode: ParseMode.Html,
                protectContent: false,
                replyParameters: new ReplyParameters
                {
                    MessageId = update.Message.MessageId
                },
                replyMarkup: new InlineKeyboardMarkup(
                    InlineKeyboardButton.WithUrl("Check sendMessage method", "https://core.telegram.org/bots/api#sendmessage")
                )
            );
        }
    }
}

