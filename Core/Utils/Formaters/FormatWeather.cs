using Core.Contracts.Models;

namespace Core.Utils.Formaters;

/// <summary>
/// Provides formatted, user-facing weather summaries optimized for chat or messaging platforms
/// (Markdown-compatible, emoji-enhanced output).
/// </summary>
/// <remarks>
/// This class is purely presentational:
/// <list type="bullet">
/// <item><description>No business logic</description></item>
/// <item><description>No data validation beyond null/empty checks</description></item>
/// <item></item><description>Output is intended for human consumption, not machine parsing</description></item>
/// </list>
/// </remarks>
public static class FormatWeather
{
    /// <summary>
    /// Builds a full, detailed weather report for a specific city.
    /// </summary>
    /// <param name="city">
    /// Name of the city being reported. Rendered prominently in the header.
    /// </param>
    /// <param name="w">
    /// Current weather snapshot containing temperature, wind, humidity,
    /// precipitation chance, UV index, and condition metadata.
    /// </param>
    /// <returns>
    /// A multi-line formatted string containing comprehensive current weather data,
    /// including feels-like temperature, cloud cover, rain chance, and UV index.
    /// </returns>
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

    /// <summary>
    /// Builds a compact current weather report without city context.
    /// </summary>
    /// <param name="w">
    /// Current weather snapshot containing essential real-time metrics.
    /// </param>
    /// <returns>
    /// A formatted string describing the current weather conditions,
    /// suitable for quick updates or replies.
    /// </returns>
    public static string Current(CurrentWeather w)
        => $@"🌦 *Current weather*

🌡 Temp: {w.TemperatureC:F1}°C
🥵 Feels like: {w.FeelsLikeC:F1}°C
💧 Humidity: {w.Humidity}%
🌬 Wind: {w.WindSpeedKph:F1} km/h
🌧 Rain: {w.RainChance}%
🔆 UV: {w.UvIndex:F1}

Condition: *{w.Condition}* {w.Icon}";

    /// <summary>
    /// Builds a short city-level weather summary with minimal details.
    /// </summary>
    /// <param name="city">
    /// Name of the city being summarized.
    /// </param>
    /// <param name="w">
    /// Current weather snapshot.
    /// </param>
    /// <returns>
    /// A lightweight formatted string showing only key metrics
    /// (temperature, humidity, wind) and condition.
    /// </returns>
    public static string CitySummary(string city, CurrentWeather w)
        => $@"🌤 Weather in *{city}*

🌡 Temp: {w.TemperatureC:F1}°C
💧 Humidity: {w.Humidity}%
🌬 Wind: {w.WindSpeedKph:F1} km/h

Condition: *{w.Condition}* {w.Icon}";

    // ─────────────────────────────────────────────
    // HOURLY
    // ─────────────────────────────────────────────

    /// <summary>
    /// Formats an hourly weather forecast for the next 12 hours.
    /// </summary>
    /// <param name="hours">
    /// Collection of hourly forecast entries. Each entry is expected to include:
    /// Time, Temperature, FeelsLike temperature, Humidity, Wind speed,
    /// Rain chance, Condition text, and an Icon.
    /// </param>
    /// <returns>
    /// A formatted string listing up to 12 upcoming hourly forecasts.
    /// If the input is null or empty, a warning message is returned instead.
    /// </returns>
    public static string Hourly(List<HourlyForecast> hours)
    {
        if (hours is null || hours.Count is 0)
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

    /// <summary>
    /// Formats a daily weather forecast for the next five days.
    /// </summary>
    /// <param name="days">
    /// Collection of daily forecast entries.
    /// Each entry is expected to contain:
    /// Date, maximum and minimum temperatures, rain chance,
    /// condition text, and an icon.
    /// </param>
    /// <returns>
    /// A formatted five-day forecast summary.
    /// If the input is null or empty, a warning message is returned.
    /// </returns>
    public static string Daily(List<DailyForecast> days)
    {
        if (days is null || days.Count is 0)
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

    /// <summary>
    /// Formats a full weekly weather forecast (up to seven days).
    /// </summary>
    /// <param name="week">
    /// Collection of daily forecast entries representing a full week.
    /// </param>
    /// <returns>
    /// A formatted weekly forecast.
    /// If the input is null or empty, a warning message is returned.
    /// </returns>
    public static string Weekly(List<DailyForecast> week)
    {
        if (week is null || week.Count is 0)
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
