using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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
    public async IAsyncEnumerable<Models.Quote> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using IAsyncCursor<Models.Quote> cursor = await _quotes.FindAsync(_ => true, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Quote quote in cursor.Current)
            {
                yield return quote;
            }
        }
    }

    /// <inheritdoc />
    public async Task<Models.Quote> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(x => x.Id, id);

        return await _quotes.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.Quote> GetByTelegramIdAsync(
        long telegramId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId);

        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(x => x.TelegramId, telegramId);

        using IAsyncCursor<Models.Quote> cursor = await _quotes.FindAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Quote quote in cursor.Current)
            {
                yield return quote;
            }
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.Quote> QueryAsync(
        Expression<Func<Models.Quote, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        using IAsyncCursor<Models.Quote> cursor = await _quotes.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Quote quote in cursor.Current)
            {
                yield return quote;
            }
        }
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
        ArgumentException.ThrowIfNullOrEmpty(entity.Id);

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _quotes.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Quote with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(id);

        FilterDefinition<Models.Quote> filter = Builders<Models.Quote>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _quotes.DeleteOneAsync(filter, cancellationToken).ConfigureAwait(false);
        return result.DeletedCount > 0;
    }
}
