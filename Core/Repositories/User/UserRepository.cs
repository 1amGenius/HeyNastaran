using System.Linq.Expressions;
using System.Runtime.CompilerServices;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Core.Repositories.User;

/// <summary>
/// MongoDB-backed implementation of the User repository.
/// </summary>
public sealed class UserRepository : IUserRepository
{
    private readonly IMongoCollection<Contracts.Models.User> _users;
        
    public UserRepository(IMongoClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));

        IMongoDatabase database = client.GetDatabase("nastaran_db");
        _users = database.GetCollection<Contracts.Models.User>("users");
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.User> GetAllAsync(
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using IAsyncCursor<Contracts.Models.User> cursor = await _users.FindAsync(_ => true, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.User user in cursor.Current)
            {
                yield return user;
            }
        }
    }

    /// <inheritdoc />
    public async Task<Contracts.Models.User> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        FilterDefinition<Contracts.Models.User> filter = Builders<Contracts.Models.User>.Filter.Eq(x => x.Id, id);

        return await _users.Find(filter).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.User> GetByTelegramIdAsync(
        long telegramId,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(telegramId, nameof(telegramId));

        FilterDefinition<Contracts.Models.User> filter = Builders<Contracts.Models.User>.Filter.Eq(x => x.TelegramId, telegramId);

        using IAsyncCursor<Contracts.Models.User> cursor = await _users.FindAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.User user in cursor.Current)
            {
                yield return user;
            }
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Contracts.Models.User> QueryAsync(
        Expression<Func<Contracts.Models.User, bool>> predicate,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));

        using IAsyncCursor<Contracts.Models.User> cursor = await _users.FindAsync(predicate, cancellationToken: cancellationToken).ConfigureAwait(false);

        while (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
        {
            foreach (Contracts.Models.User user in cursor.Current)
            {
                yield return user;
            }
        }
    }

    /// <inheritdoc />
    public async Task AddAsync(Contracts.Models.User entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));

        await _users.InsertOneAsync(entity, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Contracts.Models.User entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Contracts.Models.User> filter = Builders<Contracts.Models.User>.Filter.Eq(x => x.Id, entity.Id);

        ReplaceOneResult result = await _users.ReplaceOneAsync(filter, entity, cancellationToken: cancellationToken).ConfigureAwait(false);

        if (result.MatchedCount is 0)
        {
            throw new InvalidOperationException($"User with Id '{entity.Id}' was not found.");
        }
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(id, nameof(id));

        FilterDefinition<Contracts.Models.User> filter = Builders<Contracts.Models.User>.Filter.Eq(x => x.Id, id);

        DeleteResult result = await _users.DeleteOneAsync(filter, cancellationToken: cancellationToken).ConfigureAwait(false);
        return result.DeletedCount is > 0;
    }
}
