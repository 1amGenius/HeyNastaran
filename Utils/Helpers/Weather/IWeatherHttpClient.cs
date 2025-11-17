namespace Nastaran_bot.Utils.Helpers.Weather;

public interface IWeatherHttpClient
{
    public Task<T> GetAsync<T>(string url);
}
