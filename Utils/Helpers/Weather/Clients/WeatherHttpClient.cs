using System.Text.Json;

using Nastaran_bot.Utils.Helpers.Weather.Interfaces;

namespace Nastaran_bot.Utils.Helpers.Weather.Clients;

public class WeatherHttpClient : IWeatherHttpClient
{
    public HttpClient RawHttpClient
    {
        get;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WeatherHttpClient(HttpClient http)
    {
        RawHttpClient = http;
        RawHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 NastaranBot/1.0");
    }

    public async Task<T> GetAsync<T>(string url)
    {
        try
        {
            HttpResponseMessage response = await RawHttpClient.GetAsync(url);
            _ = response.EnsureSuccessStatusCode();

            await using Stream stream = await response.Content.ReadAsStreamAsync();
            T result = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions);

            return result ?? throw new Exception("Deserialization returned null");
        }
        catch (Exception ex)
        {
            throw new Exception($"HTTP GET failed: {url}", ex);
        }
    }
}
