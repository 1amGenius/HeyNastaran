using Nastaran_bot.Models;
using Nastaran_bot.Repositories.DailyNote;

namespace Nastaran_bot.Services.DailyNote;

public class DailyNoteService(IDailyNoteRepository dailyNoteRepository, ILogger<DailyNoteService> logger) : IDailyNoteService
{
    private readonly IDailyNoteRepository _dailyNoteRepository = dailyNoteRepository;
    private readonly ILogger<DailyNoteService> _logger = logger;

    public async Task<Models.DailyNote> AddDailyNoteAsync(long telegramId, string text, string category = "general", string author = "unknown")
    {
        try
        {
            var newNote = new Models.DailyNote
            {
                TelegramId = telegramId,
                Date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                Text = text,
                Category = category,
                Author = author,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UsedAt = null,
            };

            await _dailyNoteRepository.CreateAsync(newNote);
            return newNote;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error adding daily note for TelegramId {telegramId}", telegramId);
            throw;
        }
    }

    public async Task<IEnumerable<Models.DailyNote>> GetUserNotesAsync(long telegramId)
    {
        try
        {
            return await _dailyNoteRepository.GetByTelegramIdAsync(telegramId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching notes for TelegramId {telegramId}", telegramId);
            return [];
        }
    }

    public async Task<bool> DeleteNoteAsync(string id)
    {
        try
        {
            return await _dailyNoteRepository.DeleteAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting note {id}", id);
            return false;
        }
    }

    public async Task<Models.DailyNote> GetNoteByIdAsync(string id)
    {
        try
        {
            return await _dailyNoteRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error getting note {id}", id);
            return null;
        }
    }
}
