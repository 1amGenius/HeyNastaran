namespace Nastaran_bot.Utils.Helpers.Weather;

public interface IWeatherApiClient
{
    public Task<(double Latitude, double Longitude)> GetCoordinatesByCityNameAsync(string cityName);

    public Task<Models.Weather> GetCurrentWeatherAsync(double latitude, double longitude);
    
    public Task<Models.Weather> GetHourlyForecastAsync(double latitude, double longitude, int hours = 24);
    
    public Task<Models.Weather> GetDailyForecastAsync(double latitude, double longitude, int days = 7);
    
    public Task<Models.Weather> GetFullWeatherReportAsync(double latitude, double longitude);
     
    public Task<double> GetUvIndexAsync(double latitude, double longitude);
    
    public Task<double> GetRainChanceAsync(double latitude, double longitude);
    
    public Task<Models.Weather> GetMinimizedWeatherAsync(double latitude, double longitude);

}
