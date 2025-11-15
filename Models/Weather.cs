namespace Nastaran_bot.Models;

public class Weather
{
    public double Latitude
    {
        get;
        set;
    }

    public double Longitude
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

    public override string ToString() 
        => $"{Current.Icon} {Current.Condition}\n" +
               $"🌡 Temp: {Current.TemperatureC:F1}°C (Feels like {Current.FeelsLikeC:F1}°C)\n" +
               $"💨 Wind: {Current.WindSpeedKph:F1} km/h\n" +
               $"💧 Humidity: {Current.Humidity}%\n" +
               $"☀️ Sunrise: {Daily.FirstOrDefault()?.Sunrise}\n" +
               $"🌙 Sunset: {Daily.FirstOrDefault()?.Sunset}";
}

public class CurrentWeather
{
    public double TemperatureC
    {
        get;
        set;
    }

    public double FeelsLikeC
    {
        get;
        set;
    }

    public double WindSpeedKph
    {
        get;
        set;
    }

    public double Humidity
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
    
    public double? UvIndex
    {
        get;
        set;
    }

    public double? RainChance
    {
        get;
        set;
    }
    
    public double? CloudCover
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

    public double TemperatureC
    {
        get;
        set;
    }

    public double FeelsLikeC
    {
        get;
        set;
    }

    public double WindSpeedKph
    {
        get; 
        set;
    }

    public double Humidity
    {
        get;
        set;
    }

    public double? RainChance
    {
        get;
        set;
    }

    public double? CloudCover
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

    public double UvIndex
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

    public double TemperatureMinC
    {
        get; 
        set;
    }

    public double TemperatureMaxC
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

    public double? RainChance
    {
        get;
        set;
    }

    public double? UvIndex
    {
        get;
        set;
    }

    public double? CloudCover
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
