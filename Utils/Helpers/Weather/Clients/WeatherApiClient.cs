using Nastaran_bot.Models;
using Nastaran_bot.Utils.Helpers.Weather.Interfaces;
using Nastaran_bot.Utils.Mappers;
using Nastaran_bot.Utils.Urls;

using OpenMeteo.Geocoding;
using OpenMeteo.Weather.ResponseModel;

namespace Nastaran_bot.Utils.Helpers.Weather.Clients;

public class WeatherApiClient(IWeatherHttpClient httpClient, ILogger<WeatherApiClient> logger) : IWeatherApiClient
{
    private readonly IWeatherHttpClient _httpClient = httpClient;
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

            if (response?.Current == null)
            {
                throw new Exception("API returned no current weather data.");
            }

            Current currentApi = response.Current;
            WeatherData mapped = WeatherMapper.MapCurrentWeather(currentApi);

            (float uvIndex, int cloudCover) nearestHourly = (0f, currentApi.Cloudcover ?? 0);
            if (response.Hourly != null)
            {
                nearestHourly = GetNearestHourlyData(response.Hourly);
            }

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
                    CloudCover = nearestHourly.cloudCover,
                    UvIndex = nearestHourly.uvIndex,
                    Condition = WeatherMapper.MapCondition(mapped),
                    Icon = WeatherMapper.MapIcon(mapped)
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
            string url = WeatherApiUrls.Forecast(latitude, longitude, forecastDays: 1);
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
                var mapped = new WeatherData
                {
                    Temperature = hourlyData.Temperature_2m?[i] ?? 0f,
                    FeelsLike = hourlyData.Apparent_temperature?[i] ?? 0f,
                    Humidity = (int) (hourlyData.Relativehumidity_2m?[i] ?? 0f),
                    WindSpeed = hourlyData.Windspeed_10m?[i] ?? 0f,
                    Precipitation = hourlyData.Precipitation?[i] ?? 0f,
                    CloudCover = hourlyData.Cloudcover?[i] ?? 0,
                    IsDay = (hourlyData.Is_day?[i] ?? 0) == 1
                };

                // Use helper for this hour's nearest data if needed
                (float uvIndex, _) = GetNearestHourlyData(hourlyData);

                weather.Hourly.Add(new HourlyForecast
                {
                    Time = hourlyData.Time[i].UtcDateTime,
                    TemperatureC = mapped.Temperature,
                    FeelsLikeC = mapped.FeelsLike,
                    Humidity = mapped.Humidity,
                    WindSpeedKph = mapped.WindSpeed,
                    RainChance = mapped.Precipitation,
                    CloudCover = mapped.CloudCover,
                    UvIndex = hourlyData.Uv_index?[i] ?? uvIndex,
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
            string url = WeatherApiUrls.Forecast(latitude, longitude, forecastDays: days);
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

    // ======================================================
    // PRIVATE HELPER
    // ======================================================
    private static (float uvIndex, int cloudCover) GetNearestHourlyData(Hourly hourly)
    {
        if (hourly == null || hourly.Time == null || hourly.Uv_index == null || hourly.Cloudcover == null)
        {
            throw new Exception("Hourly weather data is incomplete.");
        }

        DateTimeOffset currentTime = DateTimeOffset.UtcNow;
        int closestIndex = 0;
        double minDiff = double.MaxValue;

        for (int i = 0; i < hourly.Time.Length; i++)
        {
            double diff = Math.Abs((hourly.Time[i] - currentTime).TotalMinutes);
            if (diff < minDiff)
            {
                minDiff = diff;
                closestIndex = i;
            }
        }

        float uvIndex = hourly.Uv_index[closestIndex].GetValueOrDefault();
        int cloudCover = hourly.Cloudcover[closestIndex].GetValueOrDefault();

        return (uvIndex, cloudCover);
    }
}
