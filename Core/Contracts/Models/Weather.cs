namespace Core.Contracts.Models;

/// <summary>
/// Represents comprehensive weather data including current, hourly, and daily information.
/// </summary>
public sealed class Weather
{
    /// <summary>
    /// Latitude of the queried location.
    /// </summary>  
    public float Latitude
    {
        get;
        set;
    }

    /// <summary>
    /// Longitude of the queried location.
    /// </summary>
    public float Longitude
    {
        get;
        set;
    }

    /// <summary>
    /// Timezone identifier corresponding to the weather data.
    /// </summary>
    public string Timezone
    {
        get;
        set;
    }

    /// <summary>
    /// Current weather conditions for the location.
    /// </summary>
    public CurrentWeather Current
    {
        get;
        set;
    } = new();

    /// <summary>
    /// Hourly weather forecasts for the location.
    /// </summary>
    public List<HourlyForecast> Hourly
    {
        get;
        set;
    } = [];

    /// <summary>
    /// Daily weather forecasts for the location.
    /// </summary>
    public List<DailyForecast> Daily
    {
        get;
        set;
    } = [];
}

/// <summary>
/// Represents the current weather conditions at a location.
/// </summary>
public class CurrentWeather
{
    /// <summary>
    /// Current temperature in Celsius.
    /// </summary>
    public float TemperatureC
    {
        get;
        set;
    }

    /// <summary>
    /// Perceived temperature in Celsius.
    /// </summary>
    public float FeelsLikeC
    {
        get;
        set;
    }

    /// <summary>
    /// Current wind speed in kilometers per hour.
    /// </summary>
    public float WindSpeedKph
    {
        get;
        set;
    }

    /// <summary>
    /// Current humidity percentage.
    /// </summary>
    public float Humidity
    {
        get;
        set;
    }

    /// <summary>
    /// Text description of current weather conditions.
    /// </summary>
    public string Condition
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Icon representing current weather conditions.
    /// </summary>
    public string Icon
    {
        get;
        set;
    } = "🌤";

    /// <summary>
    /// Current UV index, if available.
    /// </summary>
    public float? UvIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Probability of rain, if available.
    /// </summary>
    public float? RainChance
    {
        get;
        set;
    }

    /// <summary>
    /// Cloud cover percentage, if available.
    /// </summary>
    public float? CloudCover
    {
        get;
        set;
    }
}

/// <summary>
/// Represents an hourly weather forecast entry.
/// </summary>
public class HourlyForecast
{
    /// <summary>
    /// Timestamp for the forecasted hour.
    /// </summary>
    public DateTime Time
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted temperature in Celsius.
    /// </summary>
    public float TemperatureC
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted perceived temperature in Celsius.
    /// </summary>
    public float FeelsLikeC
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted wind speed in kilometers per hour.
    /// </summary>
    public float WindSpeedKph
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted humidity percentage.
    /// </summary>
    public float Humidity
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted probability of rain, if available.
    /// </summary>
    public float? RainChance
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted cloud cover percentage, if available.
    /// </summary>
    public float? CloudCover
    {
        get;
        set;
    }

    /// <summary>
    /// Text description of the forecasted conditions.
    /// </summary>
    public string Condition
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Icon representing the forecasted weather.
    /// </summary>
    public string Icon
    {
        get;
        set;
    } = "🌤";

    /// <summary>
    /// Forecasted UV index.
    /// </summary>
    public float UvIndex
    {
        get;
        set;
    }
}

/// <summary>
/// Represents a daily weather forecast entry.
/// </summary>
public class DailyForecast
{
    /// <summary>
    /// Date of the forecast.
    /// </summary>
    public DateTime Date
    {
        get;
        set;
    }

    /// <summary>
    /// Minimum forecasted temperature in Celsius.
    /// </summary>
    public float TemperatureMinC
    {
        get;
        set;
    }

    /// <summary>
    /// Maximum forecasted temperature in Celsius.
    /// </summary>
    public float TemperatureMaxC
    {
        get;
        set;
    }

    /// <summary>
    /// Sunrise time for the forecasted day.
    /// </summary>
    public DateTime Sunrise
    {
        get;
        set;
    }

    /// <summary>
    /// Sunset time for the forecasted day.
    /// </summary>
    public DateTime Sunset
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted probability of rain, if available.
    /// </summary>
    public float? RainChance
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted UV index, if available.
    /// </summary>
    public float? UvIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Forecasted cloud cover percentage, if available.
    /// </summary>
    public float? CloudCover
    {
        get;
        set;
    }

    /// <summary>
    /// Text description of the forecasted weather.
    /// </summary>
    public string Condition
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Icon representing the forecasted conditions.
    /// </summary>
    public string Icon
    {
        get;
        set;
    } = "🌤";
}
