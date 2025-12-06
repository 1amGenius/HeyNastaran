using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using MongoDB.Driver;

namespace Nastaran_bot.Repositories.Inspiration;

/// <summary>
/// MongoDB-backed implementation of the Inspiration repository.
/// </summary>
public class InspirationRepository : IInspirationRepository
{
    private readonly IMongoCollection<Models.Inspiration> _inspirations;

    public InspirationRepository(IMongoClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _inspirations = database.GetCollection<Models.Inspiration>("inspirations");
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.Inspiration> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using IAsyncCursor<Models.Inspiration> cursor = await _inspirations.FindAsync(_ => true, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Inspiration inspiration in cursor.Current)
            {
                yield return inspiration;
            }
        }
    }

    /// <inheritdoc />
    public async Task<Models.Inspiration> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        FilterDefinition<Models.Inspiration> filter = Builders<Models.Inspiration>.Filter.Eq(x => x.Id, id);

        return await _inspirations.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.Inspiration> GetByTelegramIdAsync(
        long telegramId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        FilterDefinition<Models.Inspiration> filter = Builders<Models.Inspiration>.Filter.Eq(x => x.TelegramId, telegramId);

        using IAsyncCursor<Models.Inspiration> cursor = await _inspirations.FindAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Inspiration inspiration in cursor.Current)
            {
                yield return inspiration;
            }
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.Inspiration> QueryAsync(
        Expression<Func<Models.Inspiration, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        using IAsyncCursor<Models.Inspiration> cursor = await _inspirations.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Inspiration inspiration in cursor.Current)
            {
                yield return inspiration;
            }
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(Models.Inspiration entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _inspirations.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Models.Inspiration entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.Inspiration> filter = Builders<Models.Inspiration>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _inspirations.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Inspiration with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        FilterDefinition<Models.Inspiration> filter = Builders<Models.Inspiration>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _inspirations.DeleteOneAsync(filter, cancellationToken).ConfigureAwait(false);
        return result.DeletedCount > 0;
    }
}
