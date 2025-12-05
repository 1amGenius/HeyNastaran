using System.Linq.Expressions;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

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
    public IAsyncEnumerable<Models.Idea> GetAllAsync(CancellationToken cancellationToken = default)
        => _ideas.Find(_ => true).ToAsyncEnumerable();

    /// <inheritdoc />
    public async Task<Models.Idea> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(x => x.Id, id);
        return await _ideas.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.Idea> GetByTelegramIdAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(x => x.TelegramId, telegramId);

        return _ideas.Find(filter).ToAsyncEnumerable();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.Idea> QueryAsync(
        Expression<Func<Models.Idea, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _ideas.Find(predicate).ToAsyncEnumerable();
    }

    /// <inheritdoc />
    public async Task AddAsync(Models.Idea entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _ideas.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Models.Idea entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(x => x.Id, entity.Id);
        ReplaceOneResult result = await _ideas.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Idea with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        FilterDefinition<Models.Idea> filter = Builders<Models.Idea>.Filter.Eq(x => x.Id, id);
        DeleteResult result = await _ideas.DeleteOneAsync(filter, cancellationToken);
        return result.DeletedCount > 0;
    }
}
