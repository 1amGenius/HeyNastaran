using System.Linq.Expressions;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

using Nastaran_bot.Models;

namespace Nastaran_bot.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<Models.User> _users;

    public UserRepository(IMongoClient client)
    {
        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _users = database.GetCollection<Models.User>("users");
    }

    public async Task CreateAsync(Models.User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _users.InsertOneAsync(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        DeleteResult result = await _users.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<Models.User>> FindAsync(Expression<Func<Models.User, bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await _users.AsQueryable().Where(filter).ToListAsync();
    }

    public async Task<IEnumerable<Models.User>> GetAllAsync()
        => await _users.Find(_ => true).ToListAsync();

    public async Task<Models.User> GetByIdAsync(string id)
    {
        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(n => n.Id, id);
        return await _users.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Models.User>> GetByTelegramIdAsync(long telegramId)
    {
        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq("telegramId", telegramId);
        return await _users.Find(filter).ToListAsync();
    }

    public async Task UpdateAsync(Models.User entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.User> filter = Builders<Models.User>.Filter.Eq(n => n.Id, entity.Id);
        ReplaceOneResult result = await _users.ReplaceOneAsync(filter, entity);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"User with Id {entity.Id} was not found.");
        }
    }
}
