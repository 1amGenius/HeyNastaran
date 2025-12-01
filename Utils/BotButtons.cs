using Telegram.Bot.Types.ReplyMarkups;

using static Nastaran_bot.Utils.BotButtons.Texts;

namespace Nastaran_bot.Utils;

public static class BotButtons
{
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

    public static class Actions
    {
        public static class Weather
        {
            public const string Current = "weather_current";
            public const string Hourly = "weather_hourly";
            public const string Daily = "weather_daily";
            public const string Weekly = "weather_weekly";
            public const string SearchCity = "weather_search";
        }
    }

    public static class Texts
    {
        public const string Songs = "🎵 Songs";
        public const string Quotes = "💬 Quotes";
        public const string Notes = "📝 Notes";
        public const string Ideas = "💡 Ideas";
        public const string Inspirations = "🎀 Inspirations";
        public const string Settings = "⚙ Settings";
        public const string Help = "❓ Help";

        public static class Weather
        {
            public const string _ = "🌤 Weather";
            public const string Current = "🌦 Current weather";
            public const string Hourly = "⏱ Hourly forecast";
            public const string Daily = "📆 Daily forecast";
            public const string Weekly = "📅 Weekly forecast";
            public const string SearchCity = "🔍 Search city";
        }
    }

    public static class Keyboards
    {
        public static ReplyKeyboardMarkup StartMenu => new(new[]
        {
                new KeyboardButton[]
                {
                    Songs,
                    Quotes
                },
                [
                    Weather._,
                    Notes
                ],
                [
                    Ideas,
                    Inspirations
                ],
                [
                    Settings,
                    Help
                ]
        })
        {
            ResizeKeyboard = true,
            OneTimeKeyboard = false,
            IsPersistent = true,
            InputFieldPlaceholder = "Choose an option..."
        };

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

        public static InlineKeyboardMarkup WeatherMenu => new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(Weather.Current, Actions.Weather.Current),
                InlineKeyboardButton.WithCallbackData(Weather.Hourly, Actions.Weather.Hourly)
            },
            [
                InlineKeyboardButton.WithCallbackData(Weather.Daily, Actions.Weather.Daily),
                InlineKeyboardButton.WithCallbackData(Weather.Weekly, Actions.Weather.Weekly)
            ],
            [
                InlineKeyboardButton.WithCallbackData(Weather.SearchCity, Actions.Weather.SearchCity)
            ]
        });
    }

    public static readonly IReadOnlyDictionary<string, string> GlobalButtonsToCommand = new Dictionary<string, string>
    {
        [Songs] = Commands.Songs,
        [Quotes] = Commands.Quotes,
        [Weather._] = Commands.Weather,
        [Notes] = Commands.Notes,
        [Ideas] = Commands.Ideas,
        [Inspirations] = Commands.Inspirations,
        [Settings] = Commands.Settings,
        [Help] = Commands.Help
    };

    public static readonly IReadOnlyDictionary<string, string> WeatherButtonsToAction =
    new Dictionary<string, string>
    {
        [Weather.Current] = Actions.Weather.Current,
        [Weather.Hourly] = Actions.Weather.Hourly,
        [Weather.Daily] = Actions.Weather.Daily,
        [Weather.Weekly] = Actions.Weather.Weekly,
        [Weather.SearchCity] = Actions.Weather.SearchCity
    };

}

