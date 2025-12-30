using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using MongoDB.Bson;
using MongoDB.Driver;

namespace Core.Repositories.Inspiration;

/// <summary>
/// MongoDB-backed implementation of the Inspiration repository.
/// </summary>
public sealed class InspirationRepository : IInspirationRepository
{
    private readonly IMongoCollection<Contracts.Models.Inspiration> _inspirations;

    public InspirationRepository(IMongoClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));

        IMongoDatabase database = client.GetDatabase("nastaran_db");
        _inspirations = database.GetCollection<Contracts.Models.Inspiration>("inspirations");
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Inspiration> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using IAsyncCursor<Contracts.Models.Inspiration> cursor = await _inspirations.FindAsync(_ => true, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Inspiration inspiration in cursor.Current)
            {
                yield return inspiration;
            }
        }
    }

    /// <inheritdoc />
    public async Task<Contracts.Models.Inspiration> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        FilterDefinition<Contracts.Models.Inspiration> filter = Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.Id, id);

        return await _inspirations.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Inspiration> GetByTelegramIdAsync(
        long telegramId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId, nameof(telegramId));

        FilterDefinition<Contracts.Models.Inspiration> filter = Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.TelegramId, telegramId);

        using IAsyncCursor<Contracts.Models.Inspiration> cursor = await _inspirations.FindAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Inspiration inspiration in cursor.Current)
            {
                yield return inspiration;
            }
        }
    }

    public async Task<(IReadOnlyList<Contracts.Models.Inspiration> Items, int TotalCount)> GetPageAsync(
        long userId,
        int skip,
        int take,
        CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(userId);
        ArgumentOutOfRangeException.ThrowIfNegative(skip);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(take);

        FilterDefinition<Contracts.Models.Inspiration> filter =
            Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.TelegramId, userId);

        Task<List<Contracts.Models.Inspiration>> itemsTask = _inspirations
            .Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(cancellationToken);

        Task<long> countTask = _inspirations
            .CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        await Task.WhenAll(itemsTask, countTask).ConfigureAwait(false);

        return (itemsTask.Result, (int) countTask.Result);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.Inspiration> QueryAsync(
        Expression<Func<Contracts.Models.Inspiration, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        using IAsyncCursor<Contracts.Models.Inspiration> cursor = await _inspirations.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.Inspiration inspiration in cursor.Current)
            {
                yield return inspiration;
            }
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(Contracts.Models.Inspiration entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await _inspirations.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Contracts.Models.Inspiration entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Contracts.Models.Inspiration> filter = Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _inspirations.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result.MatchedCount is 0)
        {
            throw new InvalidOperationException($"Inspiration with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public Task UpdateContentAsync(string id, string content, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        FilterDefinition<Contracts.Models.Inspiration> inspiraationFilter = Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.Id, id);

        UpdateDefinition<Contracts.Models.Inspiration> update = Builders<Contracts.Models.Inspiration>.Update
            .Set(x => x.Content, content)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        return _inspirations.UpdateOneAsync(inspiraationFilter, update, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateTagsAsync(string id, List<string> tags, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        FilterDefinition<Contracts.Models.Inspiration> inspiraationFilter = Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.Id, id);

        UpdateDefinition<Contracts.Models.Inspiration> update = Builders<Contracts.Models.Inspiration>.Update
            .Set(x => x.Tags, tags)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        return _inspirations.UpdateOneAsync(inspiraationFilter, update, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public Task UpdateLabelAsync(string id, string label, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id, nameof(id));

        FilterDefinition<Contracts.Models.Inspiration> inspiraationFilter = Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.Id, id);

        UpdateDefinition<Contracts.Models.Inspiration> update = Builders<Contracts.Models.Inspiration>.Update
            .Set(x => x.Label, label)
            .Set(x => x.UpdatedAt, DateTime.UtcNow);

        return _inspirations.UpdateOneAsync(inspiraationFilter, update, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<bool> ToggleFavoriteAsync(
        string id,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        FilterDefinition<Contracts.Models.Inspiration> filter = Builders<Contracts.Models.Inspiration>.Filter.And(
            Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.Id, id)
        );

        var update = new BsonDocument("$set", new BsonDocument
        {
            { "favorite", new BsonDocument("$not", "$favorite") },
            { "updatedAt", DateTime.UtcNow }
        });

        var options = new FindOneAndUpdateOptions<Contracts.Models.Inspiration>
        {
            ReturnDocument = ReturnDocument.After
        };

        Contracts.Models.Inspiration updated = await _inspirations.FindOneAndUpdateAsync(
            filter,
            update,
            options,
            cancellationToken
        ).ConfigureAwait(false);

        return updated?.Favorite ?? throw new InvalidOperationException("Inspiration not found.");
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        FilterDefinition<Contracts.Models.Inspiration> filter = Builders<Contracts.Models.Inspiration>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _inspirations.DeleteOneAsync(filter, cancellationToken).ConfigureAwait(false);
        return result.DeletedCount is > 0;
    }
}
