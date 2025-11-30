using Nastaran_bot.Contracts.User;
using Nastaran_bot.Models;

namespace Nastaran_bot.Utils.Mappers;

public static class UserMapper
{
    public static UserUpdateDto ToUpdateDto(User user)
        => new()
        {
            Username = user.Username,
            FirstName = user.FirstName,
            Timezone = user.Timezone,
            Location = user.Location != null
                ? new LocationDto
                {
                    Lat = user.Location.Lat,
                    Lon = user.Location.Lon,
                    City = user.Location.City,
                    Country = user.Location.Country
                }
                : null,
            Preferences = PreferencesMapper.ToDto(user.Preferences),
            FavoriteArtists = user.FavoriteArtists?.ToList(),
            IsSearchingCity = user.IsSearchingCity
        };

}
