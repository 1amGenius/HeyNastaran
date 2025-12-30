using System.Text.Json;

using Core.Utils.Helpers.Weather.Interfaces;

namespace Core.Utils.Helpers.Weather.Clients;

/// <summary>
/// Default HTTP client implementation for weather API communication.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item><description>Uses JSON deserialization with case-insensitive property matching</description></item>
/// <item><description>Ensures successful HTTP responses</description></item>
/// <item><description>Wraps transport and deserialization errors</description></item>
/// </list>
/// </remarks>
public class WeatherHttpClient : IWeatherHttpClient
{
    /// <inheritdoc />
    public HttpClient RawHttpClient
    {
        get;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherHttpClient"/> class.
    /// </summary>
    /// <param name="http">
    /// Preconfigured <see cref="HttpClient"/> instance (typically injected).
    /// </param>
    public WeatherHttpClient(HttpClient http)
    {
        RawHttpClient = http;
        RawHttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 NastaranBot/1.0");
    }

    /// <inheritdoc />
    public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        try
        {
            HttpResponseMessage response = await RawHttpClient.GetAsync(url, cancellationToken);
            _ = response.EnsureSuccessStatusCode();

            await using Stream stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            T result = await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, cancellationToken);

            return result ?? throw new NullReferenceException("Deserialization returned null");
        }
        catch (Exception ex)
        {
            throw new HttpRequestException($"HTTP GET failed: {url}", ex);
        }
    }
}
