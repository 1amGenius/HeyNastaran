namespace Nastaran_bot.Utils.Helpers.Weather.Interfaces;

public interface IWeatherHttpClient
{
    public Task<T> GetAsync<T>(string url);
    public HttpClient RawHttpClient
    {
        get;
    }
}

