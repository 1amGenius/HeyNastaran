namespace Nastaran_bot.Utils.Urls;

public static class WeatherApiUrls
{
    public const string CurrentVars =
    "temperature_2m," +                // Actual air temperature at 2 meters
    "relative_humidity_2m," +          // Humidity percentage
    "apparent_temperature," +          // Feels-like temperature
    "is_day," +                        // 1 = day, 0 = night
    "precipitation," +                 // Total precipitation
    "rain," +                          // Rain volume
    "showers," +                       // Shower volume
    "snowfall," +                      // Snowfall volume
    "weather_code," +                  // Weather condition code
    "cloudcover," +                    // Total cloud cover %
    "pressure_msl," +                  // Sea-level pressure
    "surface_pressure," +              // Local surface pressure
    "windspeed_10m," +                 // Wind speed at 10m
    "winddirection_10m," +             // Wind direction degrees
    "windgusts_10m";                   // Gust speed at 10m

    public const string HourlyVars =
    "temperature_2m," +                 // Actual air temperature at 2 meters
    "relative_humidity_2m," +           // Humidity percentage
    "apparent_temperature," +           // Feels-like temperature
    "precipitation_probability," +      // Probability of precipitation (%)
    "precipitation," +                  // Total precipitation (mm)
    "rain," +                           // Rain volume (mm)
    "showers," +                        // Shower volume (mm)
    "snowfall," +                       // Snowfall amount (cm/mm depending on API)
    "snow_depth," +                     // Snow depth on ground
    "weather_code," +                   // Weather condition code
    "cloudcover," +                     // Total cloud cover %
    "cloudcover_low," +                 // Low-level cloud cover %
    "cloudcover_mid," +                 // Mid-level cloud cover %
    "cloudcover_high," +                // High-level cloud cover %
    "visibility," +                     // Visibility
    "windspeed_10m," +                  // Wind speed at 10m height
    "winddirection_10m," +              // Wind direction in degrees
    "windgusts_10m," +                  // Wind gusts at 10m height
    "uv_index," +                       // UV index
    "uv_index_clear_sky," +             // UV index assuming clear skies
    "is_day";                           // 1 = day, 0 = night

    public const string DailyVars =
    "weather_code," +                   // Weather condition code
    "temperature_2m_max," +             // Max temp of the day
    "temperature_2m_min," +             // Min temp of the day
    "apparent_temperature_max," +       // Max feels-like temp
    "apparent_temperature_min," +       // Min feels-like temp
    "sunrise," +                        // Local sunrise time
    "sunset," +                         // Local sunset time
    "daylight_duration," +              // Duration of daylight in seconds
    "sunshine_duration," +              // Total sunny hours
    "uv_index_max," +                   // UV index average/max (API specific)
    "uv_index_clear_sky_max," +         // UV index under clear sky
    "rain_sum," +                       // Total rain for the day
    "showers_sum," +                    // Total showers for the day
    "snowfall_sum," +                   // Total snowfall for the day
    "precipitation_sum," +              // Total precipitation (all types)
    "precipitation_hours," +            // Hours with measurable precipitation
    "precipitation_probability_max," +  // Max probability of precipitation
    "windgusts_10m_max," +              // Max gusts at 10m height
    "windspeed_10m_max," +              // Max sustained wind at 10m
    "winddirection_10m_dominant," +     // Most common wind direction of the day
    "shortwave_radiation_sum," +        // Total incoming solar radiation (W/m²)
    "et0_fao_evapotranspiration";       // Moisture evaporation estimate (agriculture metric)

    // Main forecast endpoint with CURRENT + HOURLY + DAILY
    public static string Forecast(
        float lat,
        float lon,
        string current = CurrentVars,
        string hourly = HourlyVars,
        string daily = DailyVars,
        string timezone = "auto",
        int forecastDays = 7)
        => $"https://api.open-meteo.com/v1/forecast" +
           $"?latitude={lat}&longitude={lon}" +
           $"&current={current}" +
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
        => Forecast(
            lat,
            lon,
            current: CurrentVars,
            hourly: HourlyVars,
            daily: "",
            forecastDays: 1
        );
}
