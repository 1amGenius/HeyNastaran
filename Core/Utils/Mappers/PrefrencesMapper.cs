using Core.Contracts.Dtos.User;
using Core.Contracts.Models;

namespace Core.Utils.Mappers;

/// <summary>
/// Maps user preference domain models to data transfer objects (DTOs) and vice-versa.
/// </summary>
/// <remarks>
/// This mapper performs a shallow, direct property copy
/// with no validation or transformation logic.
/// </remarks>
public static class PreferencesMapper
{
    /// <summary>
    /// Converts a <see cref="Preferences"/> domain model into a <see cref="PreferencesDto"/>.
    /// </summary>
    /// <param name="p">
    /// Source preferences object.
    /// </param>
    /// <returns>
    /// A new <see cref="PreferencesDto"/> populated from the given preferences.
    /// </returns>
    public static PreferencesDto ToDto(Preferences p)
        => new()
        {
            DailyMusic = p.DailyMusic,
            DailyQuote = p.DailyQuote,
            WeatherUpdates = p.WeatherUpdates
        };

    /// <summary>
    /// Converts a <see cref="PreferencesDto"/> into a <see cref="Preferences"/> domain model.
    /// </summary>
    /// <param name="pd">
    /// Source preferences data transfer object.
    /// </param>
    /// <returns>
    /// A new <see cref="Preferences"/> populated from the given DTO.
    /// </returns>
    public static Preferences ToModel(PreferencesDto pd)
        => new()
        {
            DailyMusic = pd.DailyMusic,
            DailyQuote = pd.DailyQuote,
            WeatherUpdates = pd.WeatherUpdates
        };
}
