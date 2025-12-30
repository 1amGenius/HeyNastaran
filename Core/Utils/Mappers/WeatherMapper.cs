using Core.Contracts.Dtos.Weather;

using OpenMeteo.Weather.ResponseModel;

namespace Core.Utils.Mappers;

/// <summary>
/// Provides mapping utilities between raw weather API models,
/// internal weather data representations, and user-facing conditions.
/// </summary>
/// <remarks>
/// Centralizes all weather interpretation logic such as:
/// <list type="bullet">
/// <item><description>Temperature and precipitation extraction</description></item>
/// <item><description>Day/night awareness</description></item>
/// <item><description>Condition and icon classification</description></item>
/// </list>
/// </remarks>
public static class WeatherMapper
{
    /// <summary>
    /// Maps raw current weather API data into a normalized <see cref="WeatherData"/> model.
    /// </summary>
    /// <param name="current">
    /// Raw current weather data from the API.
    /// </param>
    /// <returns>
    /// A populated <see cref="WeatherData"/> instance, or <c>null</c> if input is null.
    /// </returns>
    public static WeatherData MapCurrentWeather(Current current)
    => current is null
        ? null
        : new WeatherData
        {
            Time = current.Time ?? DateTimeOffset.MinValue,
            Temperature = current.Temperature_2m ?? 0f,
            WeatherCode = current.Weathercode ?? 0,
            WindSpeed = current.Windspeed_10m ?? 0f,
            WindDirection = current.Winddirection_10m ?? 0,
            WindGusts = current.Windgusts_10m ?? 0f,
            Humidity = current.Relativehumidity_2m ?? 0,
            FeelsLike = current.Apparent_temperature ?? 0f,
            IsDay = current.Is_day is 1,
            Precipitation = current.Precipitation ?? 0f,
            Rain = current.Rain ?? 0f,
            Showers = current.Showers ?? 0f,
            Snowfall = current.Snowfall ?? 0f,
            CloudCover = current.Cloudcover ?? 0,
            PressureMSL = current.Pressure_msl ?? 0f,
            SurfacePressure = current.Surface_pressure ?? 0f
        };

    // ========================
    // Current Weather Mapping
    // ========================

    /// <summary>
    /// Determines a human-readable weather condition string
    /// from the current weather data.
    /// </summary>
    /// <param name="current">
    /// Normalized weather data.
    /// </param>
    /// <returns>
    /// A condition label such as "Sunny", "Cloudy", or "Rainy".
    /// Defaults to "Clear" if input is null.
    /// </returns>
    public static string MapCondition(WeatherData current)
    {
        if (current is null)
        {
            return "Clear";
        }

        float precipitation = current.Precipitation;
        int cloudCover = current.CloudCover;
        bool isDay = current.IsDay;

        return MapConditionInternal(precipitation, cloudCover, isDay);
    }

    /// <summary>
    /// Determines a weather icon based on current weather data.
    /// </summary>
    /// <param name="current">
    /// Normalized weather data.
    /// </param>
    /// <returns>
    /// An emoji representing the current weather condition.
    /// Defaults to a partly cloudy icon if input is null.
    /// </returns>
    public static string MapIcon(WeatherData current)
    {
        if (current is null)
        {
            return "🌤";
        }

        float precipitation = current.Precipitation;
        int cloudCover = current.CloudCover;
        bool isDay = current.IsDay;

        return MapIconInternal(precipitation, cloudCover, isDay);
    }

    // ========================
    // Hourly / Forecast Mapping
    // ========================

    /// <summary>
    /// Determines a condition label for forecasted weather data.
    /// </summary>
    /// <param name="precipitation">
    /// Expected precipitation amount.
    /// </param>
    /// <param name="cloudCover">
    /// Cloud cover percentage.
    /// </param>
    /// <param name="isDay">
    /// Indicates whether the forecasted time is during daytime.
    /// </param>
    /// <returns>
    /// A human-readable weather condition label.
    /// </returns>
    public static string MapCondition(float precipitation, int cloudCover, bool isDay = true)
        => MapConditionInternal(precipitation, cloudCover, isDay);

    /// <summary>
    /// Determines a weather icon for forecasted weather data.
    /// </summary>
    /// <param name="precipitation">
    /// Expected precipitation amount.
    /// </param>
    /// <param name="cloudCover">
    /// Cloud cover percentage.
    /// </param>
    /// <param name="isDay">
    /// Indicates whether the forecasted time is during daytime.
    /// </param>
    /// <returns>
    /// An emoji representing the forecasted weather condition.
    /// </returns>
    public static string MapIcon(float precipitation, int cloudCover, bool isDay = true)
        => MapIconInternal(precipitation, cloudCover, isDay);

    // ========================
    // Internal unified logic
    // ========================

    /// <summary>
    /// Core condition classification logic shared by all mapping methods.
    /// </summary>
    private static string MapConditionInternal(float precipitation, int cloudCover, bool isDay)
    {
        if (precipitation is >= 3f)
        {
            return "Rainy"; // heavy rain
        }

        if (cloudCover is >= 80)
        {
            return "Cloudy"; // mostly cloudy
        }

        if (isDay && cloudCover is <= 20)
        {
            return "Sunny"; // clear and daytime
        }

        return "Clear"; // fallback
    }

    /// <summary>
    /// Core icon selection logic shared by all mapping methods.
    /// </summary>
    private static string MapIconInternal(float precipitation, int cloudCover, bool isDay)
    {
        if (precipitation is >= 3f)
        {
            return "🌧";
        }

        if (cloudCover is >= 80)
        {
            return "☁️";
        }

        if (isDay && cloudCover is <= 20)
        {
            return "☀️";
        }

        return "🌤"; // partly cloudy fallback
    }
}
