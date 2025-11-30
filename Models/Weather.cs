namespace Nastaran_bot.Models;

public class Weather
{
    public float Latitude
    {
        get;
        set;
    }

    public float Longitude
    {
        get;
        set;
    }

    public string Timezone
    {
        get;
        set;
    }

    public CurrentWeather Current
    {
        get;
        set;
    } = new();

    public List<HourlyForecast> Hourly
    {
        get;
        set;
    } = [];

    public List<DailyForecast> Daily
    {
        get;
        set;
    } = [];
}

public class CurrentWeather
{
    public float TemperatureC
    {
        get;
        set;
    }

    public float FeelsLikeC
    {
        get;
        set;
    }

    public float WindSpeedKph
    {
        get;
        set;
    }

    public float Humidity
    {
        get;
        set;
    }

    public string Condition
    {
        get;
        set;
    } = string.Empty;

    public string Icon
    {
        get;
        set;
    } = "🌤";

    public float? UvIndex
    {
        get;
        set;
    }

    public float? RainChance
    {
        get;
        set;
    }

    public float? CloudCover
    {
        get;
        set;
    }
}

public class HourlyForecast
{
    public DateTime Time
    {
        get;
        set;
    }

    public float TemperatureC
    {
        get;
        set;
    }

    public float FeelsLikeC
    {
        get;
        set;
    }

    public float WindSpeedKph
    {
        get;
        set;
    }

    public float Humidity
    {
        get;
        set;
    }

    public float? RainChance
    {
        get;
        set;
    }

    public float? CloudCover
    {
        get;
        set;
    }

    public string Condition
    {
        get;
        set;
    } = string.Empty;

    public string Icon
    {
        get;
        set;
    } = "🌤";

    public float UvIndex
    {
        get;
        set;
    }
}

public class DailyForecast
{
    public DateTime Date
    {
        get;
        set;
    }

    public float TemperatureMinC
    {
        get;
        set;
    }

    public float TemperatureMaxC
    {
        get;
        set;
    }

    public DateTime Sunrise
    {
        get;
        set;
    }

    public DateTime Sunset
    {
        get;
        set;
    }

    public float? RainChance
    {
        get;
        set;
    }

    public float? UvIndex
    {
        get;
        set;
    }

    public float? CloudCover
    {
        get;
        set;
    }

    public string Condition
    {
        get;
        set;
    } = string.Empty;

    public string Icon
    {
        get;
        set;
    } = "🌤";
}
