using System.Text.Json;

using Core.Contracts.Dtos.Weather;
using Core.Contracts.Models;
using Core.Utils.Helpers.Weather.Interfaces;
using Core.Utils.Mappers;
using Core.Utils.Urls;

using Microsoft.Extensions.Logging;

using OpenMeteo.Geocoding;
using OpenMeteo.Weather.ResponseModel;

namespace Core.Utils.Helpers.Weather.Clients;

/// <summary>
/// Default implementation of <see cref="IWeatherApiClient"/> that communicates
/// with an external weather API via HTTP.
/// </summary>
/// <remarks>
/// Responsibilities:
/// <list type="bullet">
/// <item><description>API URL composition</description></item>
/// <item><description>HTTP execution</description></item>
/// <item><description>Response parsing and validation</description></item>
/// <item><description>Mapping API models into domain weather models</description></item>
/// </list>
/// </remarks>
public class WeatherApiClient(IWeatherHttpClient httpClient, ILogger<WeatherApiClient> logger) : IWeatherApiClient
{
    private readonly IWeatherHttpClient _httpClient = httpClient;
    private readonly ILogger<WeatherApiClient> _logger = logger;

    /// <inheritdoc />
    public async Task<(float Latitude, float Longitude)> GetCoordinatesByCityNameAsync(string cityName, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(cityName, nameof(cityName));

            string url = WeatherApiUrls.Geocoding(cityName, count: 1, language: "en");
            GeocodingApiResponse response = await _httpClient.GetAsync<GeocodingApiResponse>(url, cancellationToken);

            if (response?.Locations is null || response.Locations.Length is 0)
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

    /// <inheritdoc />
    public async Task<ReverseGeocodingResult> GetCityAndCountryByCoordinatesAsync(float latitude, float longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            string url = WeatherApiUrls.ReverseGeocoding(latitude, longitude);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent", "NastaranBot/1.0");

            HttpResponseMessage response = await _httpClient.RawHttpClient.SendAsync(request, cancellationToken);
            _ = response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(cancellationToken);

            var doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            if (!root.TryGetProperty("address", out JsonElement addr))
            {
                return new ReverseGeocodingResult("", "");
            }

            string city =
                addr.TryGetProperty("city", out JsonElement c1) ? c1.GetString() :
                addr.TryGetProperty("town", out JsonElement c2) ? c2.GetString() :
                addr.TryGetProperty("village", out JsonElement c3) ? c3.GetString() :
                addr.TryGetProperty("municipality", out JsonElement c4) ? c4.GetString() :
                "";

            string country =
                addr.TryGetProperty("country", out JsonElement ctry)
                    ? ctry.GetString()
                    : "";

            return new ReverseGeocodingResult(city ?? "", country ?? "");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Reverse geocoding failed for lat {lat}, lon {lon}",
                latitude, longitude);

            return new ReverseGeocodingResult("", "");
        }
    }

    /// <inheritdoc />
    public async Task<Contracts.Models.Weather> GetCurrentWeatherAsync(float latitude, float longitude, CancellationToken cancellationToken = default)
    {
        try
        {
            string url = WeatherApiUrls.Current(latitude, longitude);
            WeatherForecast response = await _httpClient.GetAsync<WeatherForecast>(url, cancellationToken);

            if (response?.Current is null)
            {
                throw new Exception("API returned no current weather data.");
            }

            Current currentApi = response.Current;
            WeatherData mapped = WeatherMapper.MapCurrentWeather(currentApi);

            (float uvIndex, int cloudCover) nearestHourly = (0f, currentApi.Cloudcover ?? 0);
            if (response.Hourly is not null)
            {
                nearestHourly = GetNearestHourlyData(response.Hourly);
            }

            return new Contracts.Models.Weather
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

    /// <inheritdoc />
    public async Task<Contracts.Models.Weather> GetHourlyForecastAsync(float latitude, float longitude, int hours = 24, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(1, hours, nameof(hours));
            ArgumentOutOfRangeException.ThrowIfGreaterThan(24, hours, nameof(hours));

            string url = WeatherApiUrls.Forecast(latitude, longitude, forecastDays: 1);
            WeatherForecast response = await _httpClient.GetAsync<WeatherForecast>(url, cancellationToken);

            if (response?.Hourly?.Time is null)
            {
                throw new Exception("API returned no hourly data.");
            }

            Hourly hourlyData = response.Hourly;
            int count = Math.Min(hours, hourlyData.Time.Length);

            var weather = new Contracts.Models.Weather
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

    /// <inheritdoc />
    public async Task<Contracts.Models.Weather> GetDailyForecastAsync(float latitude, float longitude, int days = 7, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(days, nameof(days));

            string url = WeatherApiUrls.Forecast(latitude, longitude, forecastDays: days);
            WeatherForecast response = await _httpClient.GetAsync<WeatherForecast>(url, cancellationToken);

            if (response?.Daily?.Time is null)
            {
                throw new Exception("API returned no daily data.");
            }

            Daily daily = response.Daily;
            Hourly hourly = response.Hourly;

            int count = Math.Min(days, daily.Time.Length);

            var weather = new Contracts.Models.Weather
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
                if (hourly?.Time is not null && hourly.Cloudcover is not null)
                {
                    var dailyHours = hourly.Time
                        .Select((t, idx) => new { Time = t, Cloud = hourly.Cloudcover[idx] ?? 0 })
                        .Where(x => x.Time.Date == date.Date)
                        .ToList();

                    if (dailyHours.Count is > 0)
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

    /// <inheritdoc />
    public async Task<Contracts.Models.Weather> GetFullWeatherReportAsync(float latitude, float longitude, CancellationToken cancellationToken = default)
    {
        Contracts.Models.Weather current = await GetCurrentWeatherAsync(latitude, longitude, cancellationToken);
        Contracts.Models.Weather hourly = await GetHourlyForecastAsync(latitude, longitude, cancellationToken: cancellationToken);
        Contracts.Models.Weather daily = await GetDailyForecastAsync(latitude, longitude, cancellationToken: cancellationToken);

        current.Hourly = hourly.Hourly;
        current.Daily = daily.Daily;

        return current;
    }

    /// <summary>
    /// Finds the hourly forecast entry closest to the current UTC time
    /// and extracts UV index and cloud cover values.
    /// </summary>
    /// <param name="hourly">Hourly forecast data set.</param>
    /// <returns>
    /// A tuple containing UV index and cloud cover for the nearest hour.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when required hourly data is missing or incomplete.
    /// </exception>
    private static (float uvIndex, int cloudCover) GetNearestHourlyData(Hourly hourly)
    {
        if (hourly is null || hourly.Time is null || hourly.Uv_index is null || hourly.Cloudcover is null)
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
