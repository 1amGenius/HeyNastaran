namespace Core.Contracts.Dtos.Weather;

/// <summary>
/// Represents a snapshot of weather conditions for a specific point in time.
/// Useful for forecasts, current weather displays, or notification logic.
/// </summary>
public sealed class WeatherData
{
    /// <summary>
    /// Timestamp of the weather observation. 
    /// </summary>
    public DateTimeOffset Time
    {
        get; 
        set;
    }

    /// <summary>
    /// Air temperature in degrees Celsius.
    /// </summary>
    public float Temperature
    {
        get;
        set;
    }

    /// <summary>
    /// Numerical code describing the general weather condition.
    /// </summary>
    public int WeatherCode
    {
        get; 
        set;
    }

    /// <summary>
    /// Wind speed measured in meters per second.
    /// </summary>
    public float WindSpeed
    {
        get; 
        set;
    }

    /// <summary>
    /// Wind direction in degrees, where 0° indicates north.
    /// </summary>
    public int WindDirection
    {
        get; 
        set;
    }

    /// <summary>
    /// Maximum expected wind gusts in meters per second.
    /// </summary>
    public float WindGusts
    {
        get; 
        set;
    }

    /// <summary>
    /// Relative humidity percentage.
    /// </summary>
    public int Humidity
    {
        get;
        set;
    }

    /// <summary>
    /// Feels-like temperature calculated using wind, humidity, and other factors.
    /// </summary>
    public float FeelsLike
    {
        get; 
        set;
    }

    /// <summary>
    /// Indicates whether the observation time falls within daylight hours.
    /// </summary>
    public bool IsDay
    {
        get;
        set;
    }

    /// <summary>
    /// Total precipitation amount in millimeters.
    /// </summary>
    public float Precipitation
    {
        get; 
        set;
    }

    /// <summary>
    /// Rainfall amount in millimeters.
    /// </summary>
    public float Rain
    {
        get; 
        set;
    }

    /// <summary>
    /// Shower precipitation amount in millimeters.
    /// </summary>
    public float Showers
    {
        get;
        set;
    }

    /// <summary>
    /// Snowfall amount in millimeters.
    /// </summary>
    public float Snowfall
    {
        get; 
        set;
    }

    /// <summary>
    /// Cloud cover percentage.
    /// </summary>
    public int CloudCover
    {
        get;
        set;
    }

    /// <summary>
    /// Atmospheric pressure at mean sea level (MSL) in hPa.
    /// </summary>
    public float PressureMSL
    {
        get; 
        set;
    }

    /// <summary>
    /// Atmospheric pressure at surface level in hPa.
    /// </summary>
    public float SurfacePressure
    {
        get;
        set;
    }
}
