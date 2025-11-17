namespace Nastaran_bot.Utils.Urls;

public static class WeatherApiUrls
{
    public static string Forecast(
        float lat,
        float lon,
        string hourly = "temperature_2m,relative_humidity_2m,weather_code,precipitation,cloudcover,windspeed_10m,uv_index",
        string timezone = "auto",
        int forecastDays = 7)
        => $"https://api.open-meteo.com/v1/forecast" +
           $"?latitude={lat}&longitude={lon}" +
           $"&hourly={hourly}" +
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

    // Optional helper for basic current data
    public static string Current(float lat, float lon)
        => $"https://api.open-meteo.com/v1/forecast" +
           $"?latitude={lat}&longitude={lon}" +
           $"&current=temperature_2m,relative_humidity_2m,weather_code,precipitation,cloudcover,windspeed_10m,uv_index" +
           $"&timezone=auto";
}
