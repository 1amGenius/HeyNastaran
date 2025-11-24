using Nastaran_bot.Repositories.Inspiration;

namespace Nastaran_bot.Services.Inspiration;

public class InspirationService(IInspirationRepository inspirationRepository, ILogger<InspirationService> logger) : IInspirationService
{
    private readonly IInspirationRepository _inspirationRepository = inspirationRepository;
    private readonly ILogger<InspirationService> _logger = logger;

    public async Task<Models.Inspiration> AddInspirationAsync(
        long telegramId,
        string caption,
        string imageFileId,
        string label,
        List<string> tags = null,
        bool favorite = false)
    {
        try
        {
            var newInspiration = new Models.Inspiration
            {
                TelegramId = telegramId,
                Content = caption,
                Favorite = favorite,
                ImageFileId = imageFileId,
                Label = label ?? string.Empty,
                Tags = tags ?? [],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _inspirationRepository.CreateAsync(newInspiration);
            return newInspiration;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding inspiration for TelegramId {telegramId}", telegramId);
            throw;
        }
    }

    public async Task<IEnumerable<Models.Inspiration>> GetUserInspirationsAsync(long telegramId)
    {
        try
        {
            return await _inspirationRepository.FindByTelegramIdAsync(telegramId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching inspirations for TelegramId {telegramId}", telegramId);
            return [];
        }
    }

    public async Task<bool> DeleteInspirationAsync(string id)
    {
        try
        {
            return await _inspirationRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting inspiration {id}", id);
            return false;
        }
    }

    public async Task<Models.Inspiration> GetInspirationByIdAsync(string id)
    {
        try
        {
            return await _inspirationRepository.FindByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting inspiration {id}", id);
            return null;
        }
    }
}
