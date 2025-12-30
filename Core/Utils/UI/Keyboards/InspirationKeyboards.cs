using Telegram.Bot.Types.ReplyMarkups;

using static Core.Utils.UI.BotButtons;

namespace Core.Utils.UI.Keyboards;

public static class InspirationKeyboards
{
    /// <summary>
    /// Inline keyboard for inspiration-related actions.
    /// </summary>
    public static InlineKeyboardMarkup InspirationsMenu => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(
                Texts.Inspirations.List,
                Actions.Inspirations.List)
        },
        [
            InlineKeyboardButton.WithCallbackData(
                Texts.Inspirations.Add,
                Actions.Inspirations.Add)
        ]
    });

    public static InlineKeyboardMarkup Single(string id, bool favorite)
        => new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    favorite ? "⭐ Unfavorite" : "☆ Favorite",
                    $"{Actions.Inspirations.ToggleFavorite}:{id}"
                ),
                InlineKeyboardButton.WithCallbackData(
                    "✏ Edit",
                    $"{Actions.Inspirations.Edit}:{id}"
                )
            },
            [
                InlineKeyboardButton.WithCallbackData(
                    "🏷 Tags",
                    $"{Actions.Inspirations.Tags}:{id}"
                ),
                InlineKeyboardButton.WithCallbackData(
                    "📂 Label",
                    $"{Actions.Inspirations.Label}:{id}"
                )
            ],
            [
                InlineKeyboardButton.WithCallbackData(
                    "🗑 Delete",
                    $"{Actions.Inspirations.DeleteConfirm}:{id}"
                )
            ]
        });

    public static InlineKeyboardMarkup ListItem(string id)
        => new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "👁 View",
                    $"{Actions.Inspirations.View}:{id}"
                ),
                InlineKeyboardButton.WithCallbackData(
                    "🗑 Delete",
                    $"{Actions.Inspirations.DeleteConfirm}:{id}"
                )
            }
        });

    public static InlineKeyboardMarkup Pagination(int page, bool hasPrev, bool hasNext)
    {
        var buttons = new List<InlineKeyboardButton>();

        if (hasPrev)
        {
            buttons.Add(
                InlineKeyboardButton.WithCallbackData(
                    "⬅ Prev",
                    $"{Actions.Inspirations.List}:{page - 1}"
                )
            );
        }

        if (hasNext)
        {
            buttons.Add(
                InlineKeyboardButton.WithCallbackData(
                    "Next ➡",
                    $"{Actions.Inspirations.List}:{page + 1}"
                )
            );
        }

        return new InlineKeyboardMarkup(buttons);
    }

    public static InlineKeyboardMarkup DeleteConfirm(string id)
        => new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(
                    "✅ Yes",
                    $"{Actions.Inspirations.Delete}:{id}"
                ),
                InlineKeyboardButton.WithCallbackData(
                    "❌ Cancel",
                    Actions.Inspirations.Cancel
                )
            }
        });

    public static InlineKeyboardMarkup Enhance(string id)
    => new(new[]
    {
        new[]
        {
            InlineKeyboardButton.WithCallbackData(
                Texts.Inspirations.Favorite,
                $"{BotButtons.Actions.Inspirations.ToggleFavorite}:{id}"
            ),
            InlineKeyboardButton.WithCallbackData(
                Texts.Inspirations.Edit,
                $"{Actions.Inspirations.Edit}:{id}"
            )
        },
        [
            InlineKeyboardButton.WithCallbackData(
                Texts.Inspirations.Tags,
                $"{Actions.Inspirations.Tags}:{id}"
            )
        ]
    });
}
