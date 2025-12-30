using Core.Services.Inspiration;
using Core.Services.TelegramBot.Interfaces;
using Core.Services.TelegramBot.State.Inspiration;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Core.Services.TelegramBot.Handlers.Updates.Inspiration;

public sealed class InspirationEditHandler(
    ITelegramBotClient botClient,
    IInspirationService inspirationService,
    InspirationEditStore editStore) : IUpdateHandler
{
    /// <inheritdoc />
    public bool CanHandle(Update update)
        => update.Message?.Text is not null
           && editStore.TryGet(update.Message.From.Id, out _);

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        long telegramId = update.Message.From.Id;
        long chatId = update.Message.Chat.Id;

        if (!editStore.TryGet(telegramId, out InspirationEditContext ctx))
        {
            return;
        }

        string text = update.Message.Text.Trim();

        switch (ctx.Field)
        {
            case EditField.Content:
                await inspirationService.UpdateContentAsync(ctx.InspirationId, text, cancellationToken);
                break;

            case EditField.Tags:
                string[] tags = text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                await inspirationService.UpdateTagsAsync(ctx.InspirationId, [.. tags], cancellationToken);
                break;

            case EditField.Label:
                await inspirationService.UpdateLabelAsync(ctx.InspirationId, text, cancellationToken);
                break;
        }

        editStore.Clear(telegramId);

        _ = await botClient.SendMessage(chatId, "✅ Inspiration updated.", cancellationToken: cancellationToken);
    }
}
