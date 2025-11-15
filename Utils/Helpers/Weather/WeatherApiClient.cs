using Nastaran_bot.Utils.Helpers.Mapper;

using OpenMeteo;
using OpenMeteo.AirQuality;
using OpenMeteo.Weather.ResponseModel;

namespace Nastaran_bot.Utils.Helpers.Weather;

public class WeatherApiClient(OpenMeteoClient client, ILogger<WeatherApiClient> logger) : IWeatherApiClient
{
    private readonly OpenMeteoClient _client = client;
    private readonly ILogger<WeatherApiClient> _logger = logger;

    // ========================
    // CURRENT WEATHER
    // ========================
    public async Task<Models.Weather> GetCurrentWeatherAsync(double latitude, double longitude)
    {
        try
        {
            WeatherForecast result = await _client.QueryWeatherApiAsync((float) latitude, (float) longitude);

            if (result?.Current == null)
            {
                throw new Exception("Weather API returned null current weather.");
            }

            WeatherData mappedCurrent = WeatherMapper.MapCurrentWeather(result.Current);

            // Fetch UV index via air quality API
            double uvIndex = await FetchCurrentUvIndex(latitude, longitude);

            return new Models.Weather
            {
                Latitude = latitude,
                Longitude = longitude,
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
            _logger.LogError(ex, "Error fetching current weather for lat={Lat}, lon={Lon}", latitude, longitude);
            throw;
        }
    }

    // ========================
    // HOURLY FORECAST
    // ========================
    public async Task<Models.Weather> GetHourlyForecastAsync(double latitude, double longitude, int hours = 24)
    {
        try
        {
            WeatherForecast result = await _client.QueryWeatherApiAsync((float) latitude, (float) longitude);

            if (result?.Hourly?.Time == null)
            {
                throw new Exception("Weather API returned null hourly forecast.");
            }

            var weather = new Models.Weather
            {
                Latitude = latitude,
                Longitude = longitude,
                Timezone = result.Timezone ?? "UTC",
                Hourly = []
            };

            int count = Math.Min(hours, result.Hourly.Time.Length);

            // Fetch hourly UV index
            float[] hourlyUv = await FetchHourlyUvIndex(latitude, longitude, count);

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
            _logger.LogError(ex, "Error fetching hourly forecast for lat={Lat}, lon={Lon}", latitude, longitude);
            throw;
        }
    }

    // ========================
    // DAILY FORECAST
    // ========================
    public async Task<Models.Weather> GetDailyForecastAsync(double latitude, double longitude, int days = 7)
    {
        try
        {
            WeatherForecast result = await _client.QueryWeatherApiAsync((float) latitude, (float) longitude);

            if (result?.Daily?.Time == null)
            {
                throw new Exception("Weather API returned null daily forecast.");
            }

            var weather = new Models.Weather
            {
                Latitude = latitude,
                Longitude = longitude,
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
            _logger.LogError(ex, "Error fetching daily forecast for lat={Lat}, lon={Lon}", latitude, longitude);
            throw;
        }
    }

    // ========================
    // FULL WEATHER REPORT
    // ========================
    public async Task<Models.Weather> GetFullWeatherReportAsync(double latitude, double longitude)
    {
        Models.Weather current = await GetCurrentWeatherAsync(latitude, longitude);
        Models.Weather hourly = await GetHourlyForecastAsync(latitude, longitude);
        Models.Weather daily = await GetDailyForecastAsync(latitude, longitude);

        current.Hourly = hourly.Hourly;
        current.Daily = daily.Daily;

        return current;
    }

    // ========================
    // UV INDEX ONLY
    // ========================
    public async Task<double> GetUvIndexAsync(double latitude, double longitude) => await FetchCurrentUvIndex(latitude, longitude);

    // ========================
    // RAIN CHANCE ONLY
    // ========================
    public async Task<double> GetRainChanceAsync(double latitude, double longitude)
    {
        Models.Weather weather = await GetCurrentWeatherAsync(latitude, longitude);
        return weather.Current.RainChance ?? 0f;
    }

    // ========================
    // MINIMAL WEATHER SUMMARY
    // ========================
    public async Task<Models.Weather> GetMinimizedWeatherAsync(double latitude, double longitude)
    {
        Models.Weather current = await GetCurrentWeatherAsync(latitude, longitude);

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
    private async Task<double> FetchCurrentUvIndex(double latitude, double longitude)
    {
        try
        {
            var options = new AirQualityOptions(latitude: (float) latitude, longitude: (float) longitude)
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

    private async Task<float[]> FetchHourlyUvIndex(double latitude, double longitude, int hours)
    {
        try
        {
            var options = new AirQualityOptions(latitude: (float) latitude, longitude: (float) longitude)
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
