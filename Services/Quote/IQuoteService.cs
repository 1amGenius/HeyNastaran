namespace Nastaran_bot.Services.Quote;

public interface IQuoteService
{
    public Task<Models.Quote> AddNoteAsync(
        long telegramId,
        string text,
        string category = "general",
        string author = "unknown");
    public Task<IEnumerable<Models.Quote>> GetUserNotesAsync(long telegramId);
    public Task<bool> DeleteNoteAsync(string id);
    public Task<Models.Quote> GetNoteByIdAsync(string id);
}
