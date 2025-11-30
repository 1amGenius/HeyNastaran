using Nastaran_bot.Models;

namespace Nastaran_bot.Utils.Formaters;

public static class FormatWeather
{
    public static string Full(string city, CurrentWeather w)
    => $@"🌤 Weather in *{city}*

🌡 Temp: {w.TemperatureC:F1}°C
🥵 Feels like: {w.FeelsLikeC:F1}°C
💧 Humidity: {w.Humidity}%
🌬 Wind: {w.WindSpeedKph:F1} km/h
☁️ Clouds: {w.CloudCover}%
🌧 Rain: {w.RainChance}%
🔆 UV: {w.UvIndex:F1}

Condition: *{w.Condition}* {w.Icon}";

    public static string Current(CurrentWeather w)
        => $@"🌦 *Current weather*

🌡 Temp: {w.TemperatureC:F1}°C
🥵 Feels like: {w.FeelsLikeC:F1}°C
💧 Humidity: {w.Humidity}%
🌬 Wind: {w.WindSpeedKph:F1} km/h
🌧 Rain: {w.RainChance}%
🔆 UV: {w.UvIndex:F1}

Condition: *{w.Condition}* {w.Icon}";

    public static string CitySummary(string city, CurrentWeather w)
        => $@"🌤 Weather in *{city}*

🌡 Temp: {w.TemperatureC:F1}°C
💧 Humidity: {w.Humidity}%
🌬 Wind: {w.WindSpeedKph:F1} km/h

Condition: *{w.Condition}* {w.Icon}";

    // ─────────────────────────────────────────────
    // HOURLY
    // ─────────────────────────────────────────────
    public static string Hourly(List<Models.HourlyForecast> hours)
    {
        if (hours == null || hours.Count == 0)
        {
            return "⚠️ No hourly forecast available.";
        }

        IEnumerable<string> lines = hours.Take(12).Select(h =>
            $@"🕒 *{h.Time:HH:mm}*
🌡 {h.TemperatureC:F1}°C (Feels {h.FeelsLikeC:F1}°C)
💧 {h.Humidity}%
🌬 {h.WindSpeedKph:F1} km/h
🌧 {h.RainChance}%
{h.Condition} {h.Icon}"
        );

        return "⏱ *Hourly forecast (next 12 hours)*\n\n" + string.Join("\n\n", lines);
    }

    // ─────────────────────────────────────────────
    // DAILY
    // ─────────────────────────────────────────────
    // expected: List<DailyWeather> where each has:
    // Date, MaxC, MinC, RainChance, Condition, Icon
    public static string Daily(List<Models.DailyForecast> days)
    {
        if (days == null || days.Count == 0)
        {
            return "⚠️ No daily forecast available.";
        }

        IEnumerable<string> lines = days.Take(5).Select(d =>
            $@"📆 *{d.Date:dddd}*
🌡 {d.TemperatureMaxC:F1}°C / {d.TemperatureMinC:F1}°C
🌧 {d.RainChance}%
{d.Condition} {d.Icon}"
        );

        return "📆 *Daily forecast (next 5 days)*\n\n" + string.Join("\n\n", lines);
    }

    // ─────────────────────────────────────────────
    // WEEKLY
    // ─────────────────────────────────────────────
    // expected: List<DailyWeather> for full week
    public static string Weekly(List<DailyForecast> week)
    {
        if (week == null || week.Count == 0)
        {
            return "⚠️ No weekly forecast available.";
        }

        IEnumerable<string> lines = week.Take(7).Select(d =>
            $@"📅 *{d.Date:ddd}*
🌡 {d.TemperatureMaxC:F1}°C / {d.TemperatureMinC:F1}°C
🌧 {d.RainChance}%
{d.Condition} {d.Icon}"
        );

        return "📅 *Weekly forecast*\n\n" + string.Join("\n\n", lines);
    }
}
