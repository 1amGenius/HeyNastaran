namespace Nastaran_bot.Utils.Urls;

public static class WeatherApiUrls
{
    public const string HourlyVars =
        "temperature_2m,relative_humidity_2m,apparent_temperature,precipitation_probability," +
        "precipitation,rain,showers,snowfall,snow_depth,weather_code,cloudcover,cloudcover_low,cloudcover_mid,cloudcover_high," +
        "windspeed_10m,winddirection_10m,windgusts_10m,uv_index,uv_index_clear_sky,is_day";

    public const string DailyVars =
        "weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min," +
        "sunrise,sunset,daylight_duration,sunshine_duration,uv_index,uv_index_clear_sky,rain_sum,showers_sum,snowfall_sum," +
        "precipitation_sum,precipitation_hours,precipitation_probability_max,windgusts_10m_max,windspeed_10m_max," +
        "dominant_wind_direction_10m,shortwave_radiation_sum,et0_fao_evapotranspiration_sum";

    // Use forecast endpoint with hourly data to get both current + nearest hourly for UV
    public static string Forecast(
        float lat,
        float lon,
        string hourly = HourlyVars,
        string daily = DailyVars,
        string timezone = "auto",
        int forecastDays = 7)
        => $"https://api.open-meteo.com/v1/forecast" +
           $"?latitude={lat}&longitude={lon}" +
           $"&hourly={hourly}" +
           $"&daily={daily}" +
           $"&timezone={timezone}" +
           $"&forecast_days={forecastDays}";

    public static string Geocoding(string cityName, int count = 1, string language = "en")
        => $"https://geocoding-api.open-meteo.com/v1/search" +
           $"?name={Uri.EscapeDataString(cityName)}" +
           $"&count={count}" +
           $"&language={language}";

    public static string AirQuality(float lat, float lon)
        => $"https://air-quality-api.open-meteo.com/v1/air-quality" +
           $"?latitude={lat}&longitude={lon}" +
           $"&hourly=uv_index,pm10,pm2_5,us_aqi";

    public static string Current(float lat, float lon)
        => Forecast(lat, lon, hourly: HourlyVars, daily: "", forecastDays: 1);

}
