using Core.Contracts.Common;
using Core.Services.Inspiration;
using Core.Services.TelegramBot.Interfaces;
using Core.Services.TelegramBot.State.Inspiration;
using Core.Utils.UI;
using Core.Utils.UI.Keyboards;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Core.Services.TelegramBot.Handlers.Updates.Inspiration;

public sealed class InspirationCallbackHandler(
    ITelegramBotClient botClient,
    IInspirationService inspirationService,
    InspirationEditStore editStore,
    InspirationCreateIntentStore createIntentStore)
    : IUpdateHandler
{
    /// <inheritdoc />
    public bool CanHandle(Update update)
        => update.CallbackQuery?.Data?.StartsWith("insp_") == true;

    /// <inheritdoc />
    public async Task HandleAsync(Update update, CancellationToken cancellationToken = default)
    {
        CallbackQuery cb = update.CallbackQuery;
        await botClient.AnswerCallbackQuery(cb.Id, cancellationToken: cancellationToken);

        long chatId = cb.Message.Chat.Id;
        long userId = cb.From.Id;
        string data = cb.Data;

        if (data is BotButtons.Actions.Inspirations.Add)
        {
            createIntentStore.Enable(userId);
            _ = await botClient.SendMessage(chatId, "📸 Send a photo with a caption.", cancellationToken: cancellationToken);
            return;
        }

        if (data.StartsWith(BotButtons.Actions.Inspirations.List))
        {
            int page = data.Contains(':') ? int.Parse(data.Split(':')[1]) : 0;
            await List(chatId, userId, page, cancellationToken);
            return;
        }

        if (data.StartsWith(BotButtons.Actions.Inspirations.View))
        {
            await ViewSingle(cb, cancellationToken);
            return;
        }

        if (data.StartsWith(BotButtons.Actions.Inspirations.Edit))
        {
            StartEdit(cb, EditField.Content);
            return;
        }

        if (data.StartsWith(BotButtons.Actions.Inspirations.Tags))
        {
            StartEdit(cb, EditField.Tags);
            return;
        }

        if (data.StartsWith(BotButtons.Actions.Inspirations.Label))
        {
            StartEdit(cb, EditField.Label);
            return;
        }

        if (data.StartsWith(BotButtons.Actions.Inspirations.ToggleFavorite))
        {
            await ToggleFavorite(cb, cancellationToken);
            return;
        }

        if (data.StartsWith(BotButtons.Actions.Inspirations.DeleteConfirm))
        {
            string id = data.Split(':')[1];
            _ = await botClient.SendMessage(chatId, "Are you sure?", replyMarkup: InspirationKeyboards.DeleteConfirm(id), cancellationToken: cancellationToken);
            return;
        }

        if (data.StartsWith(BotButtons.Actions.Inspirations.Delete))
        {
            string id = data.Split(':')[1];
            _ = await inspirationService.DeleteInspirationAsync(id, cancellationToken);
            _ = await botClient.SendMessage(chatId, "🗑 Deleted.", cancellationToken: cancellationToken);
        }
    }

    private async Task ViewSingle(CallbackQuery cb, CancellationToken cancellationToken = default)
    {
        string id = cb.Data.Split(':')[1];
        Contracts.Models.Inspiration insp = await inspirationService.GetInspirationByIdAsync(id, cancellationToken);
        if (insp is null)
        {
            return;
        }

        _ = await botClient.SendPhoto(
            cb.Message.Chat.Id,
            insp.ImageFileId, 
            caption: insp.Content,
            replyMarkup: InspirationKeyboards.Single(insp.Id, insp.Favorite),
            cancellationToken: cancellationToken);
    }

    private async Task List(long chatId, long userId, int page, CancellationToken cancellationToken = default)
    {
        PagedResult<Contracts.Models.Inspiration> result = await inspirationService.GetPageAsync(userId, page, 5, cancellationToken);

        foreach (Contracts.Models.Inspiration insp in result.Items)
        {
            _ = await botClient.SendPhoto(
                chatId,
                insp.ImageFileId,
                caption: insp.Content,
                replyMarkup: InspirationKeyboards.ListItem(insp.Id),
                cancellationToken: cancellationToken
            );
        }

        _ = await botClient.SendMessage(
            chatId,
            $"Page {page + 1}",
            replyMarkup: InspirationKeyboards.Pagination(page, result.HasPrev, result.HasNext),
            cancellationToken: cancellationToken
        );
    }

    private void StartEdit(CallbackQuery cb, EditField field)
    {
        string id = cb.Data.Split(':')[1];
        editStore.Set(cb.From.Id, new InspirationEditContext { InspirationId = id, Field = field });
    }

    private async Task ToggleFavorite(CallbackQuery cb, CancellationToken cancellationToken = default)
    {
        string id = cb.Data.Split(':')[1];
        bool fav = await inspirationService.ToggleFavoriteAsync(id, cancellationToken);

        _ = await botClient.EditMessageReplyMarkup(
            cb.Message.Chat.Id,
            cb.Message.MessageId,
            InspirationKeyboards.Single(id, fav),
            cancellationToken: cancellationToken
        );
    }
}
