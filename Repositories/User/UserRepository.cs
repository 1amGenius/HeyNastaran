using System.Linq.Expressions;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Nastaran_bot.Repositories.User;

/// <summary>
/// MongoDB-backed implementation of the User repository.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<Models.User> _users;
        
    public UserRepository(IMongoClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _users = database.GetCollection<Models.User>("users");
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.User> GetAllAsync(CancellationToken cancellationToken = default)
        => _users.Find(_ => true).ToAsyncEnumerable();

    /// <inheritdoc />
    public async Task<Models.User> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(x => x.Id, id);

        return await _users.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.User> GetByTelegramIdAsync(
        long telegramId,
        CancellationToken cancellationToken = default)
    {
        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(x => x.TelegramId, telegramId);

        return _users.Find(filter).ToAsyncEnumerable();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<Models.User> QueryAsync(
        Expression<Func<Models.User, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);
        return _users.Find(predicate).ToAsyncEnumerable();
    }

    /// <inheritdoc />
    public async Task AddAsync(Models.User entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await _users.InsertOneAsync(entity, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Models.User entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _users.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"User with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return false;
        }

        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _users.DeleteOneAsync(filter, cancellationToken: cancellationToken);
        return result.DeletedCount > 0;
    }
}
