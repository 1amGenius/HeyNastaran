using Telegram.Bot.Types.ReplyMarkups;

using static Core.Utils.UI.BotButtons;

namespace Core.Utils.UI.Keyboards;

public sealed class WeatherKeyboards
{
    /// <summary>
    /// Keyboard requesting the user's location via Telegram's native prompt.
    /// </summary>
    public static ReplyKeyboardMarkup RequestLocationMenu => new(new[]
    {
            new KeyboardButton[]
            {
                new("📍 Send my location") { RequestLocation = true }
            }
        })
    {
        ResizeKeyboard = true,
        OneTimeKeyboard = false
    };

    /// <summary>
    /// Inline keyboard for weather-related actions.
    /// </summary>
    public static InlineKeyboardMarkup WeatherMenu => new(new[]
    {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Texts.Weather.Current, Actions.Weather.Current),
                InlineKeyboardButton.WithCallbackData(Texts.Weather.Hourly, Actions.Weather.Hourly)
            },
            [
                InlineKeyboardButton.WithCallbackData(Texts.Weather.Daily, Actions.Weather.Daily),
                InlineKeyboardButton.WithCallbackData(Texts.Weather.Weekly, Actions.Weather.Weekly)
            ],
            [
                InlineKeyboardButton.WithCallbackData(Texts.Weather.SearchCity, Actions.Weather.SearchCity)
            ]
        });

    public static InlineKeyboardMarkup NotesMenu => new(new[]
    {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Texts.Notes.ViewNotes, Actions.Notes.ViewNotes),
                InlineKeyboardButton.WithCallbackData(Texts.Notes.CreateNote, Actions.Notes.CreateNote)
            },
            [
                InlineKeyboardButton.WithCallbackData(Texts.Notes.EditNote, Actions.Notes.EditNote),
                InlineKeyboardButton.WithCallbackData(Texts.Notes.DeleteNote, Actions.Notes.DeleteNote)
            ]
    });
}
