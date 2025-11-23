using Microsoft.AspNetCore.Mvc;

using Nastaran_bot.Contracts.User;
using Nastaran_bot.Services.User;

namespace Nastaran_bot.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class UsersController(
    IUserService userService
) : ControllerBase
{
    private readonly IUserService _userService = userService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
    {
        Models.User user = await _userService.AddUserAsync(dto.TelegramId, dto.Username, dto.FirstName, dto.Timezone);
        return CreatedAtAction(nameof(GetById), new
        {
            id = user.Id
        }, user);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] long? telegramId, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        IEnumerable<Models.User> users = await _userService.GetUsersAsync(telegramId, page, pageSize);
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        Models.User user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDto dto)
    {
        Models.User updated = await _userService.UpdateUserAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateLocation(string id, [FromBody] LocationDto location)
    {
        Models.User updated = await _userService.UpdateLocationAsync(id, location);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateTimezone(string id, [FromBody] string timezone)
    {
        Models.User updated = await _userService.UpdateTimezoneAsync(id, timezone);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdatePreferences(string id, [FromBody] PreferencesDto prefs)
    {
        Models.User updated = await _userService.UpdatePreferencesAsync(id, prefs);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> AddFavorite(string id, [FromBody] string artist)
    {
        Models.User updated = await _userService.AddFavoriteArtistAsync(id, artist);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFavorite(string id, [FromQuery] string artist)
    {
        Models.User updated = await _userService.RemoveFavoriteArtistAsync(id, artist);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpPost("{id}")]
    public async Task<IActionResult> SetLastCheck(string id, [FromQuery] DateTime? spotify, [FromQuery] DateTime? weather)
    {
        Models.User updated = await _userService.SetLastCheckAsync(id, spotify, weather);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        bool success = await _userService.DeleteUserAsync(id);
        return success ? NoContent() : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetUsersForNotification([FromQuery] string type, [FromQuery] string timezone, [FromQuery] int? limit)
    {
        IEnumerable<Models.User> users = await _userService.GetUsersForNotificationAsync(type, timezone, limit);
        return Ok(users);
    }
}
