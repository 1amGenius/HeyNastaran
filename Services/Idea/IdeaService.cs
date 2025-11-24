using Nastaran_bot.Repositories.Idea;

namespace Nastaran_bot.Services.Idea;

public class IdeaService(IIdeaRepository ideaRepository, ILogger<IdeaService> logger) : IIdeaService
{
    private readonly IIdeaRepository _ideaRepository = ideaRepository;
    private readonly ILogger<IdeaService> _logger = logger;

    public async Task<Models.Idea> AddIdeaAsync(
        long telegramId,
        string content,
        string label = "idea",
        List<string> tags = null,
        bool favorite = false)
    {
        try
        {
            var newIdea = new Models.Idea
            {
                TelegramId = telegramId,
                Content = content,
                Label = label,
                Tags = tags ?? [],
                Favorite = favorite,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await _ideaRepository.CreateAsync(newIdea);
            return newIdea;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding idea for TelegramId {telegramId}", telegramId);
            throw;
        }
    }

    public async Task<IEnumerable<Models.Idea>> GetUserIdeasAsync(long telegramId)
    {
        try
        {
            return await _ideaRepository.FindByTelegramIdAsync(telegramId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ideas for TelegramId {telegramId}", telegramId);
            return [];
        }
    }

    public async Task<bool> DeleteIdeaAsync(string id)
    {
        try
        {
            return await _ideaRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting idea {id}", id);
            return false;
        }
    }

    public async Task<Models.Idea> GetIdeaByIdAsync(string id)
    {
        try
        {
            return await _ideaRepository.FindByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting idea {id}", id);
            return null;
        }
    }
}
