using Core.Contracts.Dtos.User;
using Core.Contracts.Models;

namespace Core.Utils.Mappers;

/// <summary>
/// Maps user last check domain models to data transfer objects (DTOs) and vice-versa.
/// </summary>
/// <remarks>
/// This mapper performs a shallow, direct property copy
/// with no validation or transformation logic.
/// </remarks>
public static class LastCheckMapper
{
    /// <summary>
    /// Converts a <see cref="LastCheck"/> domain model into a <see cref="LastCheckDto"/>.
    /// </summary>
    /// <param name="l">
    /// Source last-check domain model.
    /// </param>
    /// <returns>
    /// A new <see cref="LastCheckDto"/> populated from the given domain model.
    /// </returns>
    public static LastCheckDto ToDto(LastCheck l)
        => new()
        {
            Spotify = l.Spotify,
            Weather = l.Weather,
        };

    /// <summary>
    /// Converts a <see cref="LastCheckDto"/> into a <see cref="LastCheck"/> domain model.
    /// </summary>
    /// <param name="ld">
    /// Source last-check data transfer object.
    /// </param>
    /// <returns>
    /// A new <see cref="LastCheck"/> populated from the given DTO.
    /// </returns>
    public static LastCheck ToModel(LastCheckDto ld)
        => new()
        {
            Spotify = ld.Spotify,
            Weather = ld.Weather,
        };
}
