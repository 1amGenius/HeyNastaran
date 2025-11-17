namespace Nastaran_bot.Utils.Helpers.Weather.Interfaces;

public interface IWeatherApiClient
{
    public Task<(float Latitude, float Longitude)> GetCoordinatesByCityNameAsync(string cityName);

    public Task<Models.Weather> GetCurrentWeatherAsync(string cityName);
    
    public Task<Models.Weather> GetHourlyForecastAsync(string cityName, int hours = 24);
    
    public Task<Models.Weather> GetDailyForecastAsync(string cityName, int days = 7);
    
    public Task<Models.Weather> GetFullWeatherReportAsync(string cityName);
}
