using System.Linq.Expressions;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Nastaran_bot.Repositories.Inspiration;

public class InspirationRepository : IInspirationRepository
{
    private readonly IMongoCollection<Models.Inspiration> _inspirations;

    public InspirationRepository(IMongoClient client)
    {
        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _inspirations = database.GetCollection<Models.Inspiration>("inspirations");
    }

    public async Task CreateAsync(Models.Inspiration entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _inspirations.InsertOneAsync(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        DeleteResult result = await _inspirations.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<Models.Inspiration>> FindAsync(Expression<Func<Models.Inspiration, bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await _inspirations.AsQueryable().Where(filter).ToListAsync();
    }

    public async Task<IEnumerable<Models.Inspiration>> FindAllAsync()
        => await _inspirations.Find(_ => true).ToListAsync();

    public async Task<Models.Inspiration> FindByIdAsync(string id)
    {
        FilterDefinition<Models.Inspiration> filter = Builders<Models.Inspiration>.Filter.Eq(n => n.Id, id);
        return await _inspirations.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Models.Inspiration>> FindByTelegramIdAsync(long telegramId)
    {
        FilterDefinition<Models.Inspiration> filter = Builders<Models.Inspiration>.Filter.Eq("telegramId", telegramId);
        return await _inspirations.Find(filter).ToListAsync();
    }

    public async Task UpdateAsync(Models.Inspiration entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.Inspiration> filter = Builders<Models.Inspiration>.Filter.Eq(n => n.Id, entity.Id);
        ReplaceOneResult result = await _inspirations.ReplaceOneAsync(filter, entity);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Inspiration with Id {entity.Id} was not found.");
        }
    }
}
