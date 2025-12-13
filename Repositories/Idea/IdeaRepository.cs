using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using MongoDB.Driver;

namespace Nastaran_bot.Repositories.Idea;

/// <summary>
/// MongoDB-backed implementation of the Idea repository.
/// </summary>
public class IdeaRepository : IIdeaRepository
{
    private readonly IMongoCollection<Models.Idea> _ideas;

    public IdeaRepository(IMongoClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _ideas = database.GetCollection<Models.Idea>("ideas");
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.Idea> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using IAsyncCursor<Models.Idea> cursor = await _ideas.FindAsync(_ => true, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Idea idea in cursor.Current)
            {
                yield return idea;
            }
        }
    }

    /// <inheritdoc />
    public async Task<Models.Idea> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(x => x.Id, id);
        
        return await _ideas.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.Idea> GetByTelegramIdAsync(
        long telegramId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId);

        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(x => x.TelegramId, telegramId);

        using IAsyncCursor<Models.Idea> cursor = await _ideas.FindAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Idea idea in cursor.Current)
            {
                yield return idea;
            }
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.Idea> QueryAsync(
        Expression<Func<Models.Idea, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        using IAsyncCursor<Models.Idea> cursor = await _ideas.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.Idea idea in cursor.Current)
            {
                yield return idea;
            }
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(Models.Idea entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _ideas.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Models.Idea entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id);

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _ideas.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Idea with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(id);

        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _ideas.DeleteOneAsync(filter, cancellationToken).ConfigureAwait(false);
        return result.DeletedCount > 0;
    }
}
