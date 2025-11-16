using System.Text.Json;

using Nastaran_bot.Models;
using Nastaran_bot.Utils.Helpers.Mapper;

using OpenMeteo;
using OpenMeteo.AirQuality;
using OpenMeteo.Geocoding;
using OpenMeteo.Weather.ResponseModel;

namespace Nastaran_bot.Utils.Helpers.Weather;

public class WeatherApiClient(OpenMeteoClient client, ILogger<WeatherApiClient> logger) : IWeatherApiClient
{
    private readonly OpenMeteoClient _client = client;
    private readonly ILogger<WeatherApiClient> _logger = logger;

    // ========================
    // GET LANGITUDE AND LONGITUDE FROM CITY NAME
    // ========================
    public async Task<(float Latitude, float Longitude)> GetCoordinatesByCityNameAsync(string cityName)
    {
        try
        {
            var geocodingOptions = new GeocodingOptions(cityName, "en", 1);
            GeocodingApiResponse response = await _client.GetLocationDataAsync(geocodingOptions);
            if (response?.Locations == null || response.Locations.Length == 0)
            {
                throw new Exception($"No geocoding results found for city: {cityName}");
            }

            LocationData location = response.Locations[0];
            return (location.Latitude, location.Longitude);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching coordinates for city: {CityName}", cityName);
            throw;
        }
    }

    // ========================
    // GET CITY NAME FROM LANGITUDE AND LONGITUDE
    // ========================
    public async Task<string> GetCityNameByCoordinatesAsync(float latitude, float longitude)
    {
        try
        {
            using var http = new HttpClient();

            string url =
                $"https://nominatim.openstreetmap.org/reverse?lat={latitude}&lon={longitude}&format=json&zoom=10&addressdetails=1";

            http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; TelegramBot/1.0)");

            string json = await http.GetStringAsync(url);

            NominatimResponse data = JsonSerializer.Deserialize<NominatimResponse>(json);

            string city = data?.Address?.City
                       ?? data?.Address?.Town
                       ?? data?.Address?.Village
                       ?? data?.Address?.Hamlet;

            if (string.IsNullOrWhiteSpace(city))
                throw new Exception("City name not found in reverse geocoding response.");

            return city;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reverse geocoding failed for lat={Lat}, lon={Lon}", latitude, longitude);
            throw;
        }
    }

    // ========================
    // CURRENT WEATHER
    // ========================
    public async Task<Models.Weather> GetCurrentWeatherAsync(string cityName)
    {
        try
        {   
            WeatherForecast result = await _client.QueryWeatherApiAsync(cityName);
            if (result?.Current == null)
            {
                throw new Exception("Weather API returned null current weather.");
            }

            WeatherData mappedCurrent = WeatherMapper.MapCurrentWeather(result.Current);

            // Fetch UV index via air quality API
            float uvIndex = await FetchCurrentUvIndex(result.Latitude, result.Longitude);

            return new Models.Weather
            {
                Latitude = result.Latitude,
                Longitude = result.Longitude,
                Timezone = result.Timezone ?? "UTC",
                Current = new Models.CurrentWeather
                {
                    TemperatureC = mappedCurrent.Temperature,
                    FeelsLikeC = mappedCurrent.FeelsLike,
                    Humidity = mappedCurrent.Humidity,
                    WindSpeedKph = mappedCurrent.WindSpeed,
                    RainChance = mappedCurrent.Precipitation,
                    CloudCover = mappedCurrent.CloudCover,
                    Condition = WeatherMapper.MapCondition(mappedCurrent),
                    Icon = WeatherMapper.MapIcon(mappedCurrent),
                    UvIndex = uvIndex
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching current weather for {cityName}", cityName);
            throw;
        }
    }

    // ========================
    // HOURLY FORECAST
    // ========================
    public async Task<Models.Weather> GetHourlyForecastAsync(string cityName, int hours = 24)
    {
        try
        {
            WeatherForecast result = await _client.QueryWeatherApiAsync(cityName);

            if (result?.Hourly?.Time == null)
            {
                throw new Exception("Weather API returned null hourly forecast.");
            }

            var weather = new Models.Weather
            {
                Latitude = result.Latitude,
                Longitude = result.Longitude,
                Timezone = result.Timezone ?? "UTC",
                Hourly = []
            };

            int count = Math.Min(hours, result.Hourly.Time.Length);

            // Fetch hourly UV index
            float[] hourlyUv = await FetchHourlyUvIndex(result.Latitude, result.Longitude, count);

            for (int i = 0; i < count; i++)
            {
                var hourlyData = new Current
                {
                    Precipitation = result.Hourly.Precipitation?[i] ?? 0f,
                    Cloudcover = result.Hourly.Cloudcover?[i] ?? 0,
                    Is_day = 1 // Assuming daytime
                };

                WeatherData mappedHourly = WeatherMapper.MapCurrentWeather(hourlyData);

                weather.Hourly.Add(new Models.HourlyForecast
                {
                    Time = result.Hourly.Time[i].DateTime,
                    TemperatureC = result.Hourly.Temperature_2m?[i] ?? 0f,
                    FeelsLikeC = result.Hourly.Apparent_temperature?[i] ?? result.Hourly.Temperature_2m?[i] ?? 0f,
                    WindSpeedKph = result.Hourly.Windspeed_10m?[i] ?? 0f,
                    Humidity = result.Hourly.Relativehumidity_2m?[i] ?? 0f,
                    RainChance = result.Hourly.Precipitation?[i] ?? 0f,
                    CloudCover = result.Hourly.Cloudcover?[i] ?? 0f,
                    Condition = WeatherMapper.MapCondition(mappedHourly),
                    Icon = WeatherMapper.MapIcon(mappedHourly),
                    UvIndex = hourlyUv.Length > i ? hourlyUv[i] : 0
                });
            }

            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching hourly forecast for {cityName}", cityName);
            throw;
        }
    }

    // ========================
    // DAILY FORECAST
    // ========================
    public async Task<Models.Weather> GetDailyForecastAsync(string cityName, int days = 7)
    {
        try
        {
            WeatherForecast result = await _client.QueryWeatherApiAsync(cityName);

            if (result?.Daily?.Time == null)
            {
                throw new Exception("Weather API returned null daily forecast.");
            }

            var weather = new Models.Weather
            {
                Latitude = result.Latitude,
                Longitude = result.Longitude,
                Timezone = result.Timezone ?? "UTC",
                Daily = []
            };

            int count = Math.Min(days, result.Daily.Time.Length);

            for (int i = 0; i < count; i++)
            {
                DateTime sunrise = DateTime.MinValue;
                DateTime sunset = DateTime.MinValue;

                if (result.Daily.Sunrise != null && i < result.Daily.Sunrise.Length)
                {
                    _ = DateTime.TryParse(result.Daily.Sunrise[i], out sunrise);
                }

                if (result.Daily.Sunset != null && i < result.Daily.Sunset.Length)
                {
                    _ = DateTime.TryParse(result.Daily.Sunset[i], out sunset);
                }

                var dailyData = new Current
                {
                    Precipitation = result.Daily.Precipitation_sum?[i] ?? 0f,
                    Cloudcover = 0,
                    Is_day = 1
                };

                WeatherData mappedDaily = WeatherMapper.MapCurrentWeather(dailyData);

                weather.Daily.Add(new Models.DailyForecast
                {
                    Date = result.Daily.Time[i].ToDateTime(TimeOnly.MinValue),
                    TemperatureMinC = result.Daily.Temperature_2m_min?[i] ?? 0f,
                    TemperatureMaxC = result.Daily.Temperature_2m_max?[i] ?? 0f,
                    Sunrise = sunrise,
                    Sunset = sunset,
                    RainChance = mappedDaily.Precipitation,
                    CloudCover = mappedDaily.CloudCover,
                    Condition = WeatherMapper.MapCondition(mappedDaily),
                    Icon = WeatherMapper.MapIcon(mappedDaily)
                });
            }

            return weather;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching daily forecast for {cityName}", cityName);
            throw;
        }
    }

    // ========================
    // FULL WEATHER REPORT
    // ========================
    public async Task<Models.Weather> GetFullWeatherReportAsync(string cityName)
    {
        Models.Weather current = await GetCurrentWeatherAsync(cityName);
        Models.Weather hourly = await GetHourlyForecastAsync(cityName);
        Models.Weather daily = await GetDailyForecastAsync(cityName);

        current.Hourly = hourly.Hourly;
        current.Daily = daily.Daily;

        return current;
    }

    // ========================
    // UV INDEX ONLY
    // ========================
    public async Task<float> GetUvIndexAsync(float latitude, float longitude) => await FetchCurrentUvIndex(latitude, longitude);

    // ========================
    // RAIN CHANCE ONLY
    // ========================
    public async Task<float> GetRainChanceAsync(string cityName)
    {
        Models.Weather weather = await GetCurrentWeatherAsync(cityName);
        return weather.Current.RainChance ?? 0f;
    }

    // ========================
    // MINIMAL WEATHER SUMMARY
    // ========================
    public async Task<Models.Weather> GetMinimizedWeatherAsync(string cityName)
    {
        Models.Weather current = await GetCurrentWeatherAsync(cityName);

        return new Models.Weather
        {
            Latitude = current.Latitude,
            Longitude = current.Longitude,
            Timezone = current.Timezone,
            Current = new Models.CurrentWeather
            {
                TemperatureC = current.Current.TemperatureC,
                FeelsLikeC = current.Current.FeelsLikeC,
                Humidity = current.Current.Humidity,
                WindSpeedKph = current.Current.WindSpeedKph,
                Condition = current.Current.Condition,
                Icon = current.Current.Icon,
                UvIndex = current.Current.UvIndex
            }
        };
    }

    // ========================
    // HELPERS FOR UV INDEX
    // ========================
    private async Task<float> FetchCurrentUvIndex(float latitude, float longitude)
    {
        try
        {
            var options = new AirQualityOptions(latitude, longitude)
            {
                Hourly = new AirQualityOptions.HourlyOptions(AirQualityOptions.HourlyOptionsParameter.uv_index)
            };

            AirQualityResponse response = await _client.QueryAirQualityAsync(options);
            return response?.Hourly?.Uv_index?.FirstOrDefault() ?? 0;
        }
        catch
        {
            return 0;
        }
    }

    private async Task<float[]> FetchHourlyUvIndex(float latitude, float longitude, int hours)
    {
        try
        {
            var options = new AirQualityOptions(latitude, longitude)
            {
                Hourly = new AirQualityOptions.HourlyOptions(AirQualityOptions.HourlyOptionsParameter.uv_index)
            };

            AirQualityResponse response = await _client.QueryAirQualityAsync(options);
            return response?.Hourly?.Uv_index?.Take(hours).Select(v => v ?? 0).ToArray() ?? new float[hours];
        }
        catch
        {
            return new float[hours];
        }
    }
}
