using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using MongoDB.Driver;

namespace Core.Repositories.Quote;

/// <summary>
/// MongoDB-backed implementation of the Quote repository.
/// </summary>
public sealed class QuoteRepository : IQuoteRepository
{
    private readonly IMongoCollection<Contracts.Models.Quote> _quotes;

    public QuoteRepository(IMongoClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));

        IMongoDatabase database = client.GetDatabase("nastaran_db");
        _quotes = database.GetCollection<Contracts.Models.Quote>("quotes");
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Quote> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using IAsyncCursor<Contracts.Models.Quote> cursor = await _quotes.FindAsync(_ => true, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Quote quote in cursor.Current)
            {
                yield return quote;
            }
        }
    }

    /// <inheritdoc />
    public async Task<Contracts.Models.Quote> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        FilterDefinition<Contracts.Models.Quote> filter = Builders<Contracts.Models.Quote>.Filter.Eq(x => x.Id, id);

        return await _quotes.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Quote> GetByTelegramIdAsync(
        long telegramId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId, nameof(telegramId));

        FilterDefinition<Contracts.Models.Quote> filter = Builders<Contracts.Models.Quote>.Filter.Eq(x => x.TelegramId, telegramId);

        using IAsyncCursor<Contracts.Models.Quote> cursor = await _quotes.FindAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Quote quote in cursor.Current)
            {
                yield return quote;
            }
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Quote> QueryAsync(
        Expression<Func<Contracts.Models.Quote, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        using IAsyncCursor<Contracts.Models.Quote> cursor = await _quotes.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Quote quote in cursor.Current)
            {
                yield return quote;
            }
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(Contracts.Models.Quote entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await _quotes.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Contracts.Models.Quote entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Contracts.Models.Quote> filter = Builders<Contracts.Models.Quote>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _quotes.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result.MatchedCount is 0)
        {
            throw new InvalidOperationException($"Quote with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        FilterDefinition<Contracts.Models.Quote> filter = Builders<Contracts.Models.Quote>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _quotes.DeleteOneAsync(filter, cancellationToken).ConfigureAwait(false);
        return result.DeletedCount is > 0;
    }
}
