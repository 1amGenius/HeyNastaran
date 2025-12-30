namespace Core.Utils.Helpers.Weather.Interfaces;

/// <summary>
/// Abstraction over HTTP communication with weather APIs.
/// </summary>
/// <remarks>
/// Allows higher-level clients to:
/// <list type="bullet">
/// <item><description>Perform typed GET requests</description></item>
/// <item><description>Access a raw <see cref="HttpClient"/> when needed</description></item>
/// </list>
/// </remarks>
public interface IWeatherHttpClient
{
    /// <summary>
    /// Executes an HTTP GET request and deserializes the JSON response into a model.
    /// </summary>
    /// <typeparam name="T">Target deserialization type.</typeparam>
    /// <param name="url">Fully-qualified request URL.</param>
    /// <param name="cancellationToken">The cancelation token used for cancelling the process</param>
    /// <returns>Deserialized response object.</returns>
    /// <exception cref="Exception">
    /// Might be thrown on deserialization failures or other unexpected responses.
    /// </exception>
    /// <exception cref="NullReferenceException">
    /// Thrown when the response content is null.
    /// </exception>
    /// <exception cref="HttpRequestException">
    /// Thrown on HTTP request failures.
    /// </exception>
    public Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default);

    /// <summary>
    /// Underlying <see cref="HttpClient"/> instance for advanced scenarios
    /// such as custom headers or manual request handling.
    /// </summary>
    public HttpClient RawHttpClient
    {
        get;
    }
}
