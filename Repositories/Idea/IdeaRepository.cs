using System.Linq.Expressions;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Nastaran_bot.Models;

namespace Nastaran_bot.Repositories.Idea;

public class IdeaRepository : IIdeaRepository
{
    private readonly IMongoCollection<Models.Idea> _ideas;

    public IdeaRepository(IMongoClient client)
    {
        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _ideas = database.GetCollection<Models.Idea>("ideas");
    }

    public async Task CreateAsync(Models.Idea entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _ideas.InsertOneAsync(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        DeleteResult result = await _ideas.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<Models.Idea>> FindAsync(Expression<Func<Models.Idea, bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await _ideas.AsQueryable().Where(filter).ToListAsync();
    }

    public async Task<IEnumerable<Models.Idea>> GetAllAsync()
        => await _ideas.Find(_ => true).ToListAsync();

    public async Task<Models.Idea> GetByIdAsync(string id)
    {
        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(n => n.Id, id);
        return await _ideas.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Models.Idea>> GetByTelegramIdAsync(long telegramId)
    {
        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq("telegramId", telegramId);
        return await _ideas.Find(filter).ToListAsync();
    }

    public async Task UpdateAsync(Models.Idea entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(n => n.Id, entity.Id);
        ReplaceOneResult result = await _ideas.ReplaceOneAsync(filter, entity);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Idea with Id {entity.Id} was not found.");
        }
    }
}
