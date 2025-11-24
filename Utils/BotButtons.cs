namespace Nastaran_bot.Utils;

public static class BotButtons
{
    public static class Commands
    {
        public const string Songs = "/songs";
        public const string Quotes = "/quotes";
        public const string Weather = "/weather";
        public const string Notes = "/notes";
        public const string Ideas = "/ideas";
        public const string Inspirations = "/inspirations";
        public const string Settings = "/settings";
        public const string Help = "/help";
    }

    public static class Texts
    {
        public const string Songs = "🎵 Songs";
        public const string Quotes = "💬 Quotes";
        public const string Weather = "🌤 Weather";
        public const string Notes = "📝 Notes";
        public const string Ideas = "💡 Ideas";
        public const string Inspirations = "🎀 Inspirations";
        public const string Settings = "⚙ Settings";
        public const string Help = "❓ Help";
    }

    public static readonly IReadOnlyDictionary<string, string> ButtonToCommand = new Dictionary<string, string>
    {
        [Texts.Songs] = Commands.Songs,
        [Texts.Quotes] = Commands.Quotes,
        [Texts.Weather] = Commands.Weather,
        [Texts.Notes] = Commands.Notes,
        [Texts.Ideas] = Commands.Ideas,
        [Texts.Inspirations] = Commands.Inspirations,
        [Texts.Settings] = Commands.Settings,
        [Texts.Help] = Commands.Help
    };
}

