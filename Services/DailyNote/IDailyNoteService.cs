using Nastaran_bot.Models;

namespace Nastaran_bot.Services.DailyNote;

public interface IDailyNoteService
{
    public Task<Models.DailyNote> AddDailyNoteAsync(long telegramId, string text, string category = "general", string author = "unknown");
    public Task<IEnumerable<Models.DailyNote>> GetUserNotesAsync(long telegramId);
    public Task<bool> DeleteNoteAsync(string id);
    public Task<Models.DailyNote> GetNoteByIdAsync(string id);
}
