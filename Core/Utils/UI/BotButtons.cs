using Telegram.Bot.Types.ReplyMarkups;

namespace Core.Utils.UI;

/// <summary>
/// Centralized definitions for bot commands, button texts,
/// callback actions, and keyboard layouts.
/// </summary>
/// <remarks>
/// This class guarantees:
/// <list type="bullet">
/// <item><description>Consistent button labeling across the bot</description></item>
/// <item><description>Single source of truth for command and callback identifiers</description></item>
/// <item><description>Safe mapping between UI buttons and bot actions</description></item>
/// </list>
/// </remarks>
public static class BotButtons
{
    /// <summary>
    /// Slash command identifiers recognized by the bot.
    /// </summary>
    public static class Commands
    {
        public const string Start = "/start";
        public const string Songs = "/songs";
        public const string Quotes = "/quotes";
        public const string Weather = "/weather";
        public const string Notes = "/notes";
        public const string Ideas = "/ideas";
        public const string Inspirations = "/inspirations";
        public const string Settings = "/settings";
        public const string Help = "/help";
    }

    /// <summary>
    /// Callback action identifiers used by inline keyboards.
    /// </summary>
    public static class Actions ////TODO: the rest enteties will be added here when implemented in the bot
    {
        /// <summary>
        /// Weather-related callback actions.
        /// </summary>
        public static class Weather
        {
            public const string Current = "weather_current";
            public const string Hourly = "weather_hourly";
            public const string Daily = "weather_daily";
            public const string Weekly = "weather_weekly";
            public const string SearchCity = "weather_search";
        }

        /// <summary>
        /// Note-related callback actions.
        /// </summary>
        public static class Notes
        {
            public const string ViewNotes = "notes_view";
            public const string CreateNote = "notes_create";
            public const string EditNote = "notes_edit";
            public const string DeleteNote = "notes_delete";
        }

        /// <summary>
        /// Inspiration-related callback actions.
        /// </summary>
        public static class Inspirations
        {
            public const string Add = "insp_add";
            public const string List = "insp_list";
            public const string View = "insp_view";

            public const string Edit = "insp_edit";
            public const string Tags = "insp_tags";
            public const string Label = "insp_label";

            public const string ToggleFavorite = "insp_fav";

            public const string DeleteConfirm = "insp_delete_confirm";
            public const string Delete = "insp_delete";

            public const string Cancel = "insp_cancel";
        }
    }

    /// <summary>
    /// Human-readable button labels shown to the user.
    /// </summary>
    public static class Texts
    {
        public const string Songs = "🎵 Songs";
        public const string Quotes = "💬 Quotes";
        public const string Ideas = "💡 Ideas";
        public const string Settings = "⚙ Settings";
        public const string Help = "❓ Help";

        /// <summary>
        /// Weather-related button labels.
        /// </summary>
        public static class Weather ////TODO: the rest enteties will have their own seperate static class too here when implemented in the bot
        {
            public const string _ = "🌤 Weather";
            public const string Current = "🌦 Current weather";
            public const string Hourly = "⏱ Hourly forecast";
            public const string Daily = "📆 Daily forecast";
            public const string Weekly = "📅 Weekly forecast";
            public const string SearchCity = "🔍 Search city";
        }

        /// <summary>
        /// Note-related button labels.
        /// </summary>
        public static class Notes
        {
            public const string _ = "📝 Notes";
            public const string ViewNotes = "📋 View existing notes";
            public const string CreateNote = "➕ Create a new note";
            public const string EditNote = "✏ Edit an existing note";
            public const string DeleteNote = "🗑 Delete a note";
        }

        /// <summary>
        /// Inspiration-related button labels.
        /// </summary>
        public static class Inspirations
        {
            public const string _ = "🎀 Inspirations";
            public const string Add = "➕ Add Inspiration";
            public const string List = "📖 List Inspirations";
            public const string Edit = "✏ Edit";
            public const string Tags = "🏷 Tags";
            public const string Label = "📂 Label";
            public const string Favorite = "⭐ Favorite";
            public const string Delete = "🗑 Delete";
            public const string Cancel = "❌ Cancel";
        }
    }

    /// <summary>
    /// Predefined reply and inline keyboard layouts.
    /// </summary>
    public static class Keyboards
    {
        /// <summary>
        /// Main persistent start menu keyboard.
        /// </summary>
        public static ReplyKeyboardMarkup StartMenu => new(new[]
        {
                new KeyboardButton[]
                {
                    Texts.Songs,
                    Texts.Weather._,
                },
                [
                    Texts.Quotes,
                    Texts.Notes._
                ],
                [
                    Texts.Ideas,
                    Texts.Inspirations._
                ],
                [
                    Texts.Settings,
                    Texts.Help
                ]
        })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false,
            IsPersistent = true,
            InputFieldPlaceholder = "Choose an option..."
        };
    }

    /// <summary>
    /// Maps global button labels to their corresponding slash commands.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> GlobalButtonsToCommand = new Dictionary<string, string>
    {
        [Texts.Songs] = Commands.Songs,
        [Texts.Quotes] = Commands.Quotes,
        [Texts.Weather._] = Commands.Weather,
        [Texts.Notes._] = Commands.Notes,
        [Texts.Ideas] = Commands.Ideas,
        [Texts.Inspirations._] = Commands.Inspirations,
        [Texts.Settings] = Commands.Settings,
        [Texts.Help] = Commands.Help
    };

    /// <summary>
    /// Maps weather button labels to inline callback actions.
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> WeatherButtonsToAction =
    new Dictionary<string, string>
    {
        [Texts.Weather.Current] = Actions.Weather.Current,
        [Texts.Weather.Hourly] = Actions.Weather.Hourly,
        [Texts.Weather.Daily] = Actions.Weather.Daily,
        [Texts.Weather.Weekly] = Actions.Weather.Weekly,
        [Texts.Weather.SearchCity] = Actions.Weather.SearchCity
    };

    public static readonly IReadOnlyDictionary<string, string> NotesButtonsToAction =
    new Dictionary<string, string>
    {
        [Texts.Notes.ViewNotes] = Actions.Notes.ViewNotes,
        [Texts.Notes.CreateNote] = Actions.Notes.CreateNote,
        [Texts.Notes.EditNote] = Actions.Notes.EditNote,
        [Texts.Notes.DeleteNote] = Actions.Notes.DeleteNote
    };

    public static readonly IReadOnlyDictionary<string, string> InspirationsButtonsToAction =
    new Dictionary<string, string>
    {
        [Texts.Inspirations.List] = Actions.Inspirations.List,
        [Texts.Inspirations.Add] = Actions.Inspirations.Add,
        [Texts.Inspirations.Delete] = Actions.Inspirations.Delete,
        [Texts.Inspirations.Favorite] = Actions.Inspirations.ToggleFavorite
    };
}

