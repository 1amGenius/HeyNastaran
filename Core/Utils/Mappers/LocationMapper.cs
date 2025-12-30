using Core.Contracts.Dtos.User;
using Core.Contracts.Models;

namespace Core.Utils.Mappers;

/// <summary>
/// Maps location domain models to data transfer objects (DTOs) and vice-versa.
/// </summary>
/// <remarks>
/// This mapper performs a shallow, direct property copy
/// with no validation or transformation logic.
/// </remarks>
public static class LocationMapper
{
    /// <summary>
    /// Converts a <see cref="Location"/> domain model into a <see cref="LocationDto"/>.
    /// </summary>
    /// <param name="l">
    /// Source location domain model.
    /// </param>
    /// <returns>
    /// A new <see cref="LocationDto"/> populated from the given domain model.
    /// </returns>
    public static LocationDto ToDto(Location l)
        => new()
        {
            City = l.City,
            Country = l.Country,
            Lat = l.Lat,
            Lon = l.Lon,
        };

    /// <summary>
    /// Converts a <see cref="LocationDto"/> into a <see cref="Location"/> domain model.
    /// </summary>
    /// <param name="ld">
    /// Source location data transfer object.
    /// </param>
    /// <returns>
    /// A new <see cref="Location"/> populated from the given DTO.
    /// </returns>
    public static Location ToModel(LocationDto ld)
        => new()
        {
            City = ld.City,
            Country = ld.Country,
            Lat = ld.Lat,
            Lon = ld.Lon,
        };
}
