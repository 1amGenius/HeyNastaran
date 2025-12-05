using System.Linq.Expressions;

using MongoDB.Driver;

namespace Nastaran_bot.Repositories.Quote;

/// <summary>
/// MongoDB-backed implementation of the Quote repository.
/// </summary>
public class QuoteRepository : IQuoteRepository
{
    private readonly IMongoCollection<Models.Quote> _quotes;

    public QuoteRepository(IMongoClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _quotes = database.GetCollection<Models.Quote>("quotes");
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.Quote> GetAllAsync(CancellationToken cancellationToken = default)
        => _quotes.Find(_ => true).ToAsyncEnumerable();

    /// <inheritdoc />
    public async Task<Models.Quote> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(x => x.Id, id);

        return await _quotes.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.Quote> GetByTelegramIdAsync(
        long telegramId,
        CancellationToken cancellationToken = default)
    {
        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(x => x.TelegramId, telegramId);

        return _quotes.Find(filter).ToAsyncEnumerable();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.Quote> QueryAsync(
        Expression<Func<Models.Quote, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _quotes.Find(predicate).ToAsyncEnumerable();
    }

    /// <inheritdoc />
    public async Task AddAsync(Models.Quote entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _quotes.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Models.Quote entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _quotes.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Quote with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _quotes.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }
}
