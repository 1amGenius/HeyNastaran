using System.Linq.Expressions;
using System.Runtime.CompilerServices;

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
    public async IAsyncEnumerable<Models.User> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using IAsyncCursor<Models.User> cursor = await _users.FindAsync(_ => true, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.User user in cursor.Current)
            {
                yield return user;
            }
        }
    }

    /// <inheritdoc />
    public async Task<Models.User> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(x => x.Id, id);

        return await _users.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.User> GetByTelegramIdAsync(
        long telegramId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId);

        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(x => x.TelegramId, telegramId);

        using IAsyncCursor<Models.User> cursor = await _users.FindAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.User user in cursor.Current)
            {
                yield return user;
            }
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Models.User> QueryAsync(
        Expression<Func<Models.User, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        using IAsyncCursor<Models.User> cursor = await _users.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Models.User user in cursor.Current)
            {
                yield return user;
            }
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(Models.User entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _users.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Models.User entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id);

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _users.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result.MatchedCount is 0)
        {
            throw new InvalidOperationException($"User with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(id);

        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _users.DeleteOneAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.DeletedCount is > 0;
    }
}
