using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using MongoDB.Driver;

namespace Core.Repositories.Idea;

/// <summary>
/// MongoDB-backed implementation of the Idea repository.
/// </summary>
public sealed class IdeaRepository : IIdeaRepository
{
    private readonly IMongoCollection<Contracts.Models.Idea> _ideas;

    public IdeaRepository(IMongoClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));

        IMongoDatabase database = client.GetDatabase("nastaran_db");
        _ideas = database.GetCollection<Contracts.Models.Idea>("ideas");
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Idea> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using IAsyncCursor<Contracts.Models.Idea> cursor = await _ideas.FindAsync(_ => true, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Idea idea in cursor.Current)
            {
                yield return idea;
            }
        }
    }

    /// <inheritdoc />
    public async Task<Contracts.Models.Idea> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        FilterDefinition<Contracts.Models.Idea> filter = Builders<Contracts.Models.Idea>.Filter.Eq(x => x.Id, id);
        
        return await _ideas.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Idea> GetByTelegramIdAsync(
        long telegramId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId, nameof(telegramId));

        FilterDefinition<Contracts.Models.Idea> filter = Builders<Contracts.Models.Idea>.Filter.Eq(x => x.TelegramId, telegramId);

        using IAsyncCursor<Contracts.Models.Idea> cursor = await _ideas.FindAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Idea idea in cursor.Current)
            {
                yield return idea;
            }
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Idea> QueryAsync(
        Expression<Func<Contracts.Models.Idea, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        using IAsyncCursor<Contracts.Models.Idea> cursor = await _ideas.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Idea idea in cursor.Current)
            {
                yield return idea;
            }
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(Contracts.Models.Idea entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await _ideas.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Contracts.Models.Idea entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Contracts.Models.Idea> filter = Builders<Contracts.Models.Idea>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _ideas.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result.MatchedCount is 0)
        {
            throw new InvalidOperationException($"Idea with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        FilterDefinition<Contracts.Models.Idea> filter = Builders<Contracts.Models.Idea>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _ideas.DeleteOneAsync(filter, cancellationToken).ConfigureAwait(false);
        return result.DeletedCount is > 0;
    }
}
