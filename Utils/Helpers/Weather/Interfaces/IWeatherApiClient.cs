using Nastaran_bot.Contracts.Weather;

namespace Nastaran_bot.Utils.Helpers.Weather.Interfaces;

public interface IWeatherApiClient
{
    public Task<(float Latitude, float Longitude)> GetCoordinatesByCityNameAsync(string cityName);

    public Task<ReverseGeocodingResult> GetCityAndCountryByCoordinatesAsync(float latitude, float longitude);

    public Task<Models.Weather> GetCurrentWeatherAsync(float latitude, float longitude);

    public Task<Models.Weather> GetHourlyForecastAsync(float latitude, float longitude, int hours = 24);

    public Task<Models.Weather> GetDailyForecastAsync(float latitude, float longitude, int days = 7);

    public Task<Models.Weather> GetFullWeatherReportAsync(float latitude, float longitude);
}
