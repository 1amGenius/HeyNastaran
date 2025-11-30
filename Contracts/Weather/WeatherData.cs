namespace Nastaran_bot.Contracts.Weather;

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
