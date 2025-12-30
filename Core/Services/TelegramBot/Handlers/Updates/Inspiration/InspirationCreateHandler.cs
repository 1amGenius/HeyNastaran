using Core.Services.Inspiration;
using Core.Services.TelegramBot.Interfaces;
using Core.Services.TelegramBot.State.Inspiration;
using Core.Utils.UI;
using Core.Utils.UI.Keyboards;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Core.Services.TelegramBot.Handlers.Updates.Inspiration;

public sealed class InspirationCreateHandler(
    ITelegramBotClient botClient,
    IInspirationService inspirationService,
    InspirationCreateIntentStore createIntentStore) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IInspirationService _inspirationService = inspirationService;
    private readonly InspirationCreateIntentStore _createIntentStore = createIntentStore;

    /// <inheritdoc />
    public bool CanHandle(Update update)
        => update.Message?.Photo?.Length is >= 1
           && !string.IsNullOrWhiteSpace(update.Message.Caption)
           && _createIntentStore.Consume(update.Message.From.Id);

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        Message message = update.Message;
        long telegramId = message.From.Id;
        long chatId = message.Chat.Id;

        PhotoSize photo = message.Photo.Last(); 

        Contracts.Models.Inspiration created = await _inspirationService.AddInspirationAsync(
            telegramId,
            message.Caption,
            photo.FileId,
            cancellationToken: cancellationToken
        );

        _ = await _botClient.SendMessage(
            chatId,
            "✅ Inspiration saved. You can enhance it:",
            replyMarkup: InspirationKeyboards.Enhance(created.Id),
            cancellationToken: cancellationToken
        );
    }
}
