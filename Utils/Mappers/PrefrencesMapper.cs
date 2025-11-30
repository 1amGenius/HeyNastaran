using Nastaran_bot.Contracts.User;
using Nastaran_bot.Models;

namespace Nastaran_bot.Utils.Mappers;

public static class PreferencesMapper
{
    public static PreferencesDto ToDto(Preferences p)
        => new()
        {
            DailyMusic = p.DailyMusic,
            DailyQuote = p.DailyQuote,
            WeatherUpdates = p.WeatherUpdates
        };
}
