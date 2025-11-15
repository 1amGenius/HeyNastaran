namespace Nastaran_bot.Utils.Helpers.Mapper;

using OpenMeteo.Weather.ResponseModel;

public class WeatherData
{
    public DateTimeOffset Time
    {
        get; set;
    }
    public float Temperature
    {
        get; set;
    }
    public int WeatherCode
    {
        get; set;
    }
    public float WindSpeed
    {
        get; set;
    }
    public int WindDirection
    {
        get; set;
    }
    public float WindGusts
    {
        get; set;
    }
    public int Humidity
    {
        get; set;
    }
    public float FeelsLike
    {
        get; set;
    }
    public bool IsDay
    {
        get; set;
    }
    public float Precipitation
    {
        get; set;
    }
    public float Rain
    {
        get; set;
    }
    public float Showers
    {
        get; set;
    }
    public float Snowfall
    {
        get; set;
    }
    public int CloudCover
    {
        get; set;
    }
    public float PressureMSL
    {
        get; set;
    }
    public float SurfacePressure
    {
        get; set;
    }
}

public static class WeatherMapper
{
    public static WeatherData MapCurrentWeather(Current current)
    => current == null
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
            IsDay = current.Is_day == 1,
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
    public static string MapCondition(WeatherData current)
    {
        if (current == null)
        {
            return "Clear";
        }

        float precipitation = current.Precipitation;
        int cloudCover = current.CloudCover;
        bool isDay = current.IsDay;

        return MapConditionInternal(precipitation, cloudCover, isDay);
    }

    public static string MapIcon(WeatherData current)
    {
        if (current == null)
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
    public static string MapCondition(float precipitation, int cloudCover, bool isDay = true) 
        => MapConditionInternal(precipitation, cloudCover, isDay);

    public static string MapIcon(float precipitation, int cloudCover, bool isDay = true) 
        => MapIconInternal(precipitation, cloudCover, isDay);

    // ========================
    // Internal unified logic
    // ========================
    private static string MapConditionInternal(float precipitation, int cloudCover, bool isDay)
    {
        if (precipitation >= 3f)
        {
            return "Rainy"; // heavy rain
        }

        if (cloudCover >= 80)
        {
            return "Cloudy"; // mostly cloudy
        }

        if (isDay && cloudCover <= 20)
        {
            return "Sunny"; // clear and daytime
        }

        return "Clear"; // fallback
    }

    private static string MapIconInternal(float precipitation, int cloudCover, bool isDay)
    {
        if (precipitation >= 3f)
            return "🌧";
        if (cloudCover >= 80)
            return "☁️";
        if (isDay && cloudCover <= 20)
            return "☀️";
        return "🌤"; // partly cloudy fallback
    }
}
