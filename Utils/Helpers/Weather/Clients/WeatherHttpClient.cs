using System.Text.Json;

using Nastaran_bot.Utils.Helpers.Weather.Interfaces;

namespace Nastaran_bot.Utils.Helpers.Weather.Clients;

public class WeatherHttpClient : IWeatherHttpClient
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public WeatherHttpClient(HttpClient http)
    {
        _http = http;
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 WeatherBot/1.0");
    }

    public async Task<T> GetAsync<T>(string url)
    {
        try
        {
            HttpResponseMessage response = await _http.GetAsync(url);
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
