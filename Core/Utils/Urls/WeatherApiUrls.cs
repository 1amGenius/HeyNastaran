namespace Core.Utils.Urls;

/// <summary>
/// Centralized factory for constructing Open-Meteo and OpenStreetMap Nominatim API URLs.
/// </summary>
/// <remarks>
/// This class:
/// <list type="bullet">
/// <item><description>Defines reusable query parameter sets for current, hourly, and daily data</description></item>
/// <item><description>Ensures consistency across all weather API calls</description></item>
/// <item><description>Avoids hardcoded URLs scattered throughout the codebase</description></item>
/// </list>
/// </remarks>
public static class WeatherApiUrls
{
    /// <summary>
    /// Query variables used for retrieving current weather data.
    /// </summary>
    /// <remarks>
    /// These fields are optimized for real-time weather display and mapping
    /// to <see cref="Contracts.Models.CurrentWeather"/> and <see cref="Contracts.Dtos.Weather.WeatherData"/>.
    /// </remarks>
    public const string CurrentVars =
    "temperature_2m,"                 +      // Actual air temperature at 2 meters
    "relative_humidity_2m,"           +      // Humidity percentage
    "apparent_temperature,"           +      // Feels-like temperature
    "is_day,"                         +      // 1 = day, 0 = night
    "precipitation,"                  +      // Total precipitation
    "rain,"                           +      // Rain volume
    "showers,"                        +      // Shower volume
    "snowfall,"                       +      // Snowfall volume
    "weather_code,"                   +      // Weather condition code
    "cloudcover,"                     +      // Total cloud cover %
    "pressure_msl,"                   +      // Sea-level pressure
    "surface_pressure,"               +      // Local surface pressure
    "windspeed_10m,"                  +      // Wind speed at 10m
    "winddirection_10m,"              +      // Wind direction degrees
    "windgusts_10m";                         // Gust speed at 10m

    /// <summary>
    /// Query variables used for retrieving hourly forecast data.
    /// </summary>
    /// <remarks>
    /// Includes extended atmospheric, visibility, UV, and cloud-layer data
    /// required for detailed hourly forecasts and trend analysis.
    /// </remarks>
    public const string HourlyVars =
    "temperature_2m,"                 +      // Actual air temperature at 2 meters
    "relative_humidity_2m,"           +      // Humidity percentage
    "apparent_temperature,"           +      // Feels-like temperature
    "precipitation_probability,"      +      // Probability of precipitation (%)
    "precipitation,"                  +      // Total precipitation (mm)
    "rain,"                           +      // Rain volume (mm)
    "showers,"                        +      // Shower volume (mm)
    "snowfall,"                       +      // Snowfall amount (cm/mm depending on API)
    "snow_depth,"                     +      // Snow depth on ground
    "weather_code,"                   +      // Weather condition code
    "cloudcover,"                     +      // Total cloud cover %
    "cloudcover_low,"                 +      // Low-level cloud cover %
    "cloudcover_mid,"                 +      // Mid-level cloud cover %
    "cloudcover_high,"                +      // High-level cloud cover %
    "visibility,"                     +      // Visibility
    "windspeed_10m,"                  +      // Wind speed at 10m height
    "winddirection_10m,"              +      // Wind direction in degrees
    "windgusts_10m,"                  +      // Wind gusts at 10m height
    "uv_index,"                       +      // UV index
    "uv_index_clear_sky,"             +      // UV index assuming clear skies
    "is_day";                                // 1 = day, 0 = night

    /// <summary>
    /// Query variables used for retrieving daily forecast data.
    /// </summary>
    /// <remarks>
    /// Focused on daily aggregates, astronomical data, UV exposure,
    /// and precipitation summaries.
    /// </remarks>
    public const string DailyVars =
    "weather_code,"                   +      // Weather condition code
    "temperature_2m_max,"             +      // Max temp of the day
    "temperature_2m_min,"             +      // Min temp of the day
    "apparent_temperature_max,"       +      // Max feels-like temp
    "apparent_temperature_min,"       +      // Min feels-like temp
    "sunrise,"                        +      // Local sunrise time
    "sunset,"                         +      // Local sunset time
    "daylight_duration,"              +      // Duration of daylight in seconds
    "sunshine_duration,"              +      // Total sunny hours
    "uv_index_max,"                   +      // UV index average/max (API specific)
    "uv_index_clear_sky_max,"         +      // UV index under clear sky
    "rain_sum,"                       +      // Total rain for the day
    "showers_sum,"                    +      // Total showers for the day
    "snowfall_sum,"                   +      // Total snowfall for the day
    "precipitation_sum,"              +      // Total precipitation (all types)
    "precipitation_hours,"            +      // Hours with measurable precipitation
    "precipitation_probability_max,"  +      // Max probability of precipitation
    "windgusts_10m_max,"              +      // Max gusts at 10m height
    "windspeed_10m_max,"              +      // Max sustained wind at 10m
    "winddirection_10m_dominant,"     +      // Most common wind direction of the day
    "shortwave_radiation_sum,"        +      // Total incoming solar radiation (W/m²)
    "et0_fao_evapotranspiration";            // Moisture evaporation estimate (agriculture metric)

    /// <summary>
    /// Builds the main forecast API URL including current, hourly,
    /// and daily weather data.
    /// </summary>
    /// <param name="lat">Latitude coordinate.</param>
    /// <param name="lon">Longitude coordinate.</param>
    /// <param name="current">Comma-separated current weather variables. Defaults to <see cref="CurrentVars"/></param>
    /// <param name="hourly">Comma-separated hourly forecast variables. Defaults to <see cref="HourlyVars"/></param>
    /// <param name="daily">Comma-separated daily forecast variables. Defaults to <see cref="DailyVars"/></param>
    /// <param name="timezone">
    /// Timezone handling mode. Defaults to <c>"auto"</c>.
    /// </param>
    /// <param name="forecastDays">
    /// Number of days to include in the forecast. Defaults to <c>7</c> (whole week).
    /// </param>
    /// <returns>
    /// Fully-qualified Open-Meteo forecast API URL.
    /// </returns>
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

    /// <summary>
    /// Builds a geocoding API URL for resolving coordinates from a city name.
    /// </summary>
    /// <param name="cityName">City name to search for.</param>
    /// <param name="count">Maximum number of results to return. Defaults to <c>1</c></param>
    /// <param name="language">Language code for results. Defaults to <c>"en"</c></param>
    /// <returns>
    /// Fully-qualified geocoding API URL.
    /// </returns>
    public static string Geocoding(string cityName, int count = 1, string language = "en")
        => $"https://geocoding-api.open-meteo.com/v1/search" +
           $"?name={Uri.EscapeDataString(cityName)}" +
           $"&count={count}" +
           $"&language={language}";

    /// <summary>
    /// Builds an air quality API URL for a specific location.
    /// </summary>
    /// <param name="lat">Latitude coordinate.</param>
    /// <param name="lon">Longitude coordinate.</param>
    /// <returns>
    /// Fully-qualified air quality API URL.
    /// </returns>
    public static string AirQuality(float lat, float lon)
        => $"https://air-quality-api.open-meteo.com/v1/air-quality" +
           $"?latitude={lat}&longitude={lon}" +
           $"&hourly=uv_index,pm10,pm2_5,us_aqi";

    /// <summary>
    /// Builds a forecast URL optimized for retrieving only current weather data.
    /// </summary>
    /// <param name="lat">Latitude coordinate.</param>
    /// <param name="lon">Longitude coordinate.</param>
    /// <returns>
    /// Forecast API URL containing current and minimal hourly data.
    /// </returns>
    public static string Current(float lat, float lon)
        => Forecast(
            lat,
            lon,
            current: CurrentVars,
            hourly: HourlyVars,
            daily: "",
            forecastDays: 1
        );

    /// <summary>
    /// Builds a reverse geocoding URL using OpenStreetMap Nominatim.
    /// </summary>
    /// <param name="lat">Latitude coordinate.</param>
    /// <param name="lon">Longitude coordinate.</param>
    /// <param name="format">Response format. Defaults to <c>"json"</c>.</param>
    /// <param name="zoom">
    /// Detail level of returned address information. Defaults to <c>10</c>.
    /// </param>
    /// <returns>
    /// Fully-qualified reverse geocoding API URL.
    /// </returns>
    public static string ReverseGeocoding(float lat, float lon, string format = "json", int zoom = 10) 
        => $"https://nominatim.openstreetmap.org/reverse" +
           $"?format={format}&lat={lat}&lon={lon}" +
           $"&zoom={zoom}&addressdetails=1";

}
