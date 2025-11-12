namespace Nastaran_bot.Repositories.DailyNote;

public interface IDailyNoteRepository : IRepository<Models.DailyNote>
{
    public Task<IEnumerable<Models.DailyNote>> GetByTelegramIdAsync(long telegramId);
}
