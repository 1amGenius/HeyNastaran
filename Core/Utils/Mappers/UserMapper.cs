using Core.Contracts.Dtos.User;
using Core.Contracts.Models;

namespace Core.Utils.Mappers;

/// <summary>
/// Maps user domain models to DTOs used for update or persistence operations.
/// </summary>
/// <remarks>
/// This mapper:
/// <list type="bullet">
/// <item><description>Flattens nested objects</description></item>
/// <item><description>Handles optional location data</description></item>
/// <item><description>Delegates preference mapping to <see cref="PreferencesMapper"/></description></item>
/// </list>
/// </remarks>
public static class UserMapper
{
    /// <summary>
    /// Converts a <see cref="User"/> domain model into a <see cref="UserUpdateDto"/>.
    /// </summary>
    /// <param name="user">
    /// Source user entity.
    /// </param>
    /// <returns>
    /// A <see cref="UserUpdateDto"/> containing the updatable user fields.
    /// </returns>
    public static UserUpdateDto ToUpdateDto(User user)
        => new()
        {
            Username = user.Username,
            FirstName = user.FirstName,
            Timezone = user.Timezone,
            Location = user.Location is not null
                ? LocationMapper.ToDto(user.Location)
                : null,
            Preferences = user.Preferences is not null 
                ? PreferencesMapper.ToDto(user.Preferences)
                : null,
            LastCkeck = user.LastCheck is not null
                ? LastCheckMapper.ToDto(user.LastCheck) 
                : null,
            FavoriteArtists = user.FavoriteArtists?.ToList(),
            IsSearchingCity = user.IsSearchingCity
        };
}
