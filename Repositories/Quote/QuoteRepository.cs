using System.Linq.Expressions;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Nastaran_bot.Repositories.Quote;

public class QuoteRepository : IQuoteRepository
{
    private readonly IMongoCollection<Models.Quote> _quotes;

    public QuoteRepository(IMongoClient client)
    {
        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _quotes = database.GetCollection<Models.Quote>("quotes");
    }

    public async Task CreateAsync(Models.Quote entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _quotes.InsertOneAsync(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        DeleteResult result = await _quotes.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<Models.Quote>> FindAsync(Expression<Func<Models.Quote, bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await _quotes.AsQueryable().Where(filter).ToListAsync();
    }

    public async Task<IEnumerable<Models.Quote>> FindAllAsync()
        => await _quotes.Find(_ => true).ToListAsync();

    public async Task<Models.Quote> FindByIdAsync(string id)
    {
        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(n => n.Id, id);
        return await _quotes.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Models.Quote>> FindByTelegramIdAsync(long telegramId)
    {
        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq("telegramId", telegramId);
        return await _quotes.Find(filter).ToListAsync();
    }

    public async Task UpdateAsync(Models.Quote entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(n => n.Id, entity.Id);
        ReplaceOneResult result = await _quotes.ReplaceOneAsync(filter, entity);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Quote with Id {entity.Id} was not found.");
        }
    }
}
