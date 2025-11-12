namespace Nastaran_bot.Services.Idea;

public interface IIdeaService
{
    public Task<Models.Idea> AddIdeaAsync(
        long telegramId,
        string content,
        string label = "idea",
        List<string> tags = null,
        bool favorite = false);
    public Task<IEnumerable<Models.Idea>> GetUserIdeasAsync(long telegramId);
    public Task<bool> DeleteIdeaAsync(string id);
    public Task<Models.Idea> GetIdeaByIdAsync(string id);
}
