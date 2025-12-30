using Core.Contracts.Dtos.Weather;
using Core.Contracts.Models;

namespace Core.Utils.Helpers.Weather.Interfaces;

/// <summary>
/// Defines a high-level contract for retrieving geolocation and weather data
/// from an external weather provider. (Open-Meteo or OpenStreetMap Nominatim)
/// </summary>
/// <remarks>
/// Implementations are responsible for:
/// <list type="bullet">
/// <item><description>Calling remote APIs</description></item>
/// <item><description>Mapping raw API responses into domain models</description></item>
/// <item><description>Handling network and deserialization failures</description></item>
/// </list>
/// </remarks>
public interface IWeatherApiClient
{
    /// <summary>
    /// Resolves geographic coordinates (latitude, longitude) from a city name.
    /// </summary>
    /// <param name="cityName">
    /// Human-readable city name (e.g., "Berlin", "Tehran").
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used for canceling the process
    /// </param>
    /// <returns>
    /// A tuple containing latitude and longitude.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="cityName"/> is null or empty.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the API returns no matching locations or the request fails.
    /// </exception>
    public Task<(float Latitude, float Longitude)> GetCoordinatesByCityNameAsync(string cityName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves city and country information from geographic coordinates.
    /// </summary>
    /// <param name="latitude">Latitude value.</param>
    /// <param name="longitude">Longitude value.</param>
    /// <param name="cancellationToken">
    /// The cancellation token used for canceling the process
    /// </param>
    /// <returns>
    /// A <see cref="ReverseGeocodingResult"/> containing city and country names.
    /// Empty strings are returned if reverse geocoding fails.
    /// </returns>
    public Task<ReverseGeocodingResult> GetCityAndCountryByCoordinatesAsync(float latitude, float longitude, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the current weather conditions for a specific location.
    /// </summary>
    /// <param name="latitude">Latitude value.</param>
    /// <param name="longitude">Longitude value.</param>
    /// <param name="cancellationToken">
    /// The cancellation token used for canceling the process
    /// </param>
    /// <returns>
    /// A <see cref="Contracts.Models.Weather"/> object containing populated
    /// <see cref="CurrentWeather"/> data.
    /// </returns>
    /// <exception cref="Exception">
    /// Thrown when the API request fails or returns invalid data.
    /// </exception>
    public Task<Contracts.Models.Weather> GetCurrentWeatherAsync(float latitude, float longitude, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an hourly weather forecast for a specific location.
    /// </summary>
    /// <param name="latitude">Latitude value.</param>
    /// <param name="longitude">Longitude value.</param>
    /// <param name="hours">
    /// Number of hours to return. Defaults to 24 and is capped by API availability.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used for canceling the process
    /// </param>
    /// <returns>
    /// A <see cref="Contracts.Models.Weather"/> object containing hourly forecast entries.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="hours"/> is less than 1 or greater than 24.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the API request fails or returns invalid data.
    /// </exception>
    public Task<Contracts.Models.Weather> GetHourlyForecastAsync(float latitude, float longitude, int hours = 24, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a daily weather forecast for a specific location.
    /// </summary>
    /// <param name="latitude">Latitude value.</param>
    /// <param name="longitude">Longitude value.</param>
    /// <param name="days">
    /// Number of forecast days to return. Defaults to 7.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token used for canceling the process
    /// </param>
    /// <returns>
    /// A <see cref="Contracts.Models.Weather"/> object containing daily forecast entries.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="days"/> is 0 or negative.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown when the API request fails or returns invalid data.
    /// </exception>
    public Task<Contracts.Models.Weather> GetDailyForecastAsync(float latitude, float longitude, int days = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a combined weather report containing current, hourly,
    /// and daily forecasts.
    /// </summary>
    /// <param name="latitude">Latitude value.</param>
    /// <param name="longitude">Longitude value.</param>
    /// <param name="cancellationToken">
    /// The cancellation token used for canceling the process
    /// </param>
    /// <returns>
    /// A fully populated <see cref="Contracts.Models.Weather"/> object.
    /// </returns>
    public Task<Contracts.Models.Weather> GetFullWeatherReportAsync(float latitude, float longitude, CancellationToken cancellationToken = default);
}
