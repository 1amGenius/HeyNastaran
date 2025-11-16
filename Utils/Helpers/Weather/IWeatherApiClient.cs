namespace Nastaran_bot.Utils.Helpers.Weather;

public interface IWeatherApiClient
{
    public Task<string> GetCityNameByCoordinatesAsync(float latitude, float longitude);

    public Task<(float Latitude, float Longitude)> GetCoordinatesByCityNameAsync(string cityName);

    public Task<Models.Weather> GetCurrentWeatherAsync(string cityName);
    
    public Task<Models.Weather> GetHourlyForecastAsync(string cityName, int hours = 24);
    
    public Task<Models.Weather> GetDailyForecastAsync(string cityName, int days = 7);
    
    public Task<Models.Weather> GetFullWeatherReportAsync(string cityName);
     
    public Task<float> GetUvIndexAsync(float latitude, float longitude);
    
    public Task<float> GetRainChanceAsync(string cityName);
    
    public Task<Models.Weather> GetMinimizedWeatherAsync(string cityName);

}
