namespace Nastaran_bot.Services.Inspiration;

public interface IInspirationService
{
    public Task<Models.Inspiration> AddInspirationAsync(
        long telegramId,
        string caption,
        string imageFileId,
        string label = "mood_board",
        List<string> tags = null,
        bool favorite = false);
    public Task<IEnumerable<Models.Inspiration>> GetUserInspirationsAsync(long telegramId);
    public Task<bool> DeleteInspirationAsync(string id);
    public Task<Models.Inspiration> GetInspirationByIdAsync(string id);
}
