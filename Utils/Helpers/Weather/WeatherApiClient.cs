namespace Nastaran_bot.Utils.Helpers.Weather;

public class WeatherApiClient : IWeatherApiClient
{
    public Task<Models.Weather> GetCurrentWeatherAsync(double latitude, double longitude) => throw new NotImplementedException();

    public Task<Models.Weather> GetDailyForecastAsync(double latitude, double longitude, int days = 7) => throw new NotImplementedException();
    
    public Task<Models.Weather> GetFullWeatherReportAsync(double latitude, double longitude) => throw new NotImplementedException();
    
    public Task<Models.Weather> GetHourlyForecastAsync(double latitude, double longitude, int hours = 24) => throw new NotImplementedException();
    
    public Task<Models.Weather> GetMinimizedWeatherAsync(double latitude, double longitude) => throw new NotImplementedException();
    
    public Task<double> GetRainChanceAsync(double latitude, double longitude) => throw new NotImplementedException();

    public Task<double> GetUvIndexAsync(double latitude, double longitude) => throw new NotImplementedException();
}
