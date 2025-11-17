using Nastaran_bot.Models;
using Nastaran_bot.Utils.Helpers.Weather.Interfaces;
using Nastaran_bot.Utils.Mapper;
using Nastaran_bot.Utils.Urls;

using OpenMeteo.Geocoding;
using OpenMeteo.Weather.ResponseModel;

namespace Nastaran_bot.Utils.Helpers.Weather.Clients;

public class WeatherApiClient(WeatherHttpClient httpClient, ILogger<WeatherApiClient> logger) : IWeatherApiClient
{
    private readonly WeatherHttpClient _httpClient = httpClient;
    private readonly ILogger<WeatherApiClient> _logger = logger;

    // ======================================================
    // GEOCODING – GET COORDINATES FROM CITY NAME
    // ======================================================
    public async Task<(float Latitude, float Longitude)> GetCoordinatesByCityNameAsync(string cityName)
    {
        try
        {
            string url = WeatherApiUrls.Geocoding(cityName, count: 1, language: "en");

            GeocodingApiResponse response = await _httpClient.GetAsync<GeocodingApiResponse>(url);
            if (response?.Locations == null || response.Locations.Length == 0)
            {
                throw new Exception($"No location results found for city: {cityName}");
            }

            LocationData loc = response.Locations[0];
            return ((float) loc.Latitude, (float) loc.Longitude);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching coordinates for city: {cityName}", cityName);
            throw;
        }
    }

    // ======================================================
    // CURRENT WEATHER
    // ======================================================
    public async Task<Models.Weather> GetCurrentWeatherAsync(float latitude, float longitude)
    {
        try
        {
            string url = WeatherApiUrls.Current(latitude, longitude);

            WeatherForecast response = await _httpClient.GetAsync<WeatherForecast>(url);

            if (response?.Current == null || response.Hourly == null || response.Hourly.Time == null || response.Hourly.Uv_index == null || response.Hourly.Cloudcover == null)
            {
                throw new Exception("API returned incomplete current weather or hourly data.");
            }

            WeatherData mapped = WeatherMapper.MapCurrentWeather(response.Current);

            DateTimeOffset currentTime = DateTimeOffset.UtcNow;

            int closestIndex = 0;
            double minDiff = double.MaxValue;
            for (int i = 0; i < response.Hourly.Time.Length; i++)
            {
                DateTimeOffset t = response.Hourly.Time[i];
                double diff = Math.Abs((t - currentTime).TotalMinutes);

                if (diff < minDiff)
                {
                    minDiff = diff;
                    closestIndex = i;
                }
            }

            float uvIndex = response.Hourly.Uv_index[closestIndex].Value;
            int cloudCover = response.Hourly.Cloudcover[closestIndex].Value;
            bool isDay = (response.Hourly.Is_day?[closestIndex] ?? 0) == 1;

            return new Models.Weather
            {
                Latitude = latitude,
                Longitude = longitude,
                Timezone = response.Timezone ?? "UTC",
                Current = new CurrentWeather
                {
                    TemperatureC = mapped.Temperature,
                    FeelsLikeC = mapped.FeelsLike,
                    Humidity = mapped.Humidity,
                    WindSpeedKph = mapped.WindSpeed,
                    RainChance = mapped.Precipitation,
                    CloudCover = cloudCover,
                    Condition = WeatherMapper.MapCondition(mapped),
                    Icon = WeatherMapper.MapIcon(mapped),
                    UvIndex = uvIndex
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current weather for lat {lat} lon {lon}", latitude, longitude);
            throw;
        }
    }

    // ======================================================
    // HOURLY FORECAST
    // ======================================================
    public async Task<Models.Weather> GetHourlyForecastAsync(float latitude, float longitude, int hours = 24)
    {
        try
        {
            string url = WeatherApiUrls.Forecast(
                latitude,
                longitude,
                hourly: "temperature_2m,apparent_temperature,relative_humidity_2m,weather_code,precipitation,cloudcover,windspeed_10m,uv_index,is_day",
                forecastDays: 1
            );

            WeatherForecast response = await _httpClient.GetAsync<WeatherForecast>(url);

            if (response?.Hourly?.Time == null)
            {
                throw new Exception("API returned no hourly data.");
            }

            Hourly hourlyData = response.Hourly;
            int count = Math.Min(hours, hourlyData.Time.Length);

            var weather = new Models.Weather
            {
                Latitude = latitude,
                Longitude = longitude,
                Timezone = response.Timezone ?? "UTC",
                Hourly = []
            };

            for (int i = 0; i < count; i++)
            {
                float temp = hourlyData.Temperature_2m?[i] ?? 0f;
                float feels = hourlyData.Apparent_temperature?[i] ?? temp;
                float humidity = hourlyData.Relativehumidity_2m?[i] ?? 0f;
                float windSpeed = hourlyData.Windspeed_10m?[i] ?? 0f;
                float precipitation = hourlyData.Precipitation?[i] ?? 0f;
                int cloudCover = hourlyData.Cloudcover?[i] ?? 0;
                float uvIndex = hourlyData.Uv_index?[i] ?? 0f;
                bool isDay = (hourlyData.Is_day?[i] ?? 0) == 1;

                var mapped = new WeatherData
                {
                    Temperature = temp,
                    FeelsLike = feels,
                    Humidity = (int) humidity,
                    WindSpeed = windSpeed,
                    Precipitation = precipitation,
                    CloudCover = cloudCover,
                    IsDay = isDay
                };

                weather.Hourly.Add(new HourlyForecast
                {
                    Time = hourlyData.Time[i].UtcDateTime,
                    TemperatureC = temp,
                    FeelsLikeC = feels,
                    Humidity = (int) humidity,
                    WindSpeedKph = windSpeed,
                    RainChance = precipitation,
                    CloudCover = cloudCover,
                    UvIndex = uvIndex,
                    Condition = WeatherMapper.MapCondition(mapped),
                    Icon = WeatherMapper.MapIcon(mapped)
                });
            }

            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching hourly forecast for lat {lat} lon {lon}", latitude, longitude);
            throw;
        }
    }

    // ======================================================
    // DAILY FORECAST
    // ======================================================
    public async Task<Models.Weather> GetDailyForecastAsync(float latitude, float longitude, int days = 7)
    {
        try
        {
            string url = WeatherApiUrls.Forecast(
                latitude,
                longitude,
                hourly: "cloudcover,temperature_2m,apparent_temperature,relative_humidity_2m,precipitation,windspeed_10m,uv_index,is_day",
                forecastDays: days
            );

            WeatherForecast response = await _httpClient.GetAsync<WeatherForecast>(url);

            if (response?.Daily?.Time == null)
            {
                throw new Exception("API returned no daily data.");
            }

            Daily daily = response.Daily;
            Hourly hourly = response.Hourly; 

            int count = Math.Min(days, daily.Time.Length);

            var weather = new Models.Weather
            {
                Latitude = latitude,
                Longitude = longitude,
                Timezone = response.Timezone ?? "UTC",
                Daily = []
            };

            for (int i = 0; i < count; i++)
            {
                var date = daily.Time[i].ToDateTime(TimeOnly.MinValue);

                DateTime sunrise = DateTimeOffset.TryParse(daily.Sunrise?[i], out DateTimeOffset s1) ? s1.DateTime : DateTime.MinValue;
                DateTime sunset = DateTimeOffset.TryParse(daily.Sunset?[i], out DateTimeOffset s2) ? s2.DateTime : DateTime.MinValue;

                float precipitation = daily.Precipitation_sum?[i] ?? 0f;

                int cloudCover = 0;
                if (hourly?.Time != null && hourly.Cloudcover != null)
                {
                    var dailyHours = hourly.Time
                        .Select((t, idx) => new { Time = t, Cloud = hourly.Cloudcover[idx] ?? 0 })
                        .Where(x => x.Time.Date == date.Date)
                        .ToList();

                    if (dailyHours.Count > 0)
                    {
                        cloudCover = (int) Math.Round(dailyHours.Average(x => x.Cloud));
                    }
                }

                weather.Daily.Add(new DailyForecast
                {
                    Date = date,
                    TemperatureMinC = daily.Temperature_2m_min?[i] ?? 0f,
                    TemperatureMaxC = daily.Temperature_2m_max?[i] ?? 0f,
                    Sunrise = sunrise,
                    Sunset = sunset,
                    RainChance = precipitation,
                    CloudCover = cloudCover,
                    Condition = WeatherMapper.MapCondition(precipitation, cloudCover, true),
                    Icon = WeatherMapper.MapIcon(precipitation, cloudCover, true)
                });
            }

            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching daily forecast for lat {lat} lon {lon}", latitude, longitude);
            throw;
        }
    }

    // ======================================================
    // COMBINED WEATHER REPORT
    // ======================================================
    public async Task<Models.Weather> GetFullWeatherReportAsync(float latitude, float longitude)
    {
        Models.Weather current = await GetCurrentWeatherAsync(latitude, longitude);
        Models.Weather hourly = await GetHourlyForecastAsync(latitude, longitude);
        Models.Weather daily = await GetDailyForecastAsync(latitude, longitude);

        current.Hourly = hourly.Hourly;
        current.Daily = daily.Daily;

        return current;
    }
}
