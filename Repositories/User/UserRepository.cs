using System.Linq.Expressions;

using MongoDB.Driver;

using Nastaran_bot.Models;

namespace Nastaran_bot.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<Users> _users;

    public UserRepository(IMongoClient client)
    {
        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _users = database.GetCollection<Users>("users");
    }

    public Task CreateAsync(Users entity) => throw new NotImplementedException();

    public Task DeleteAsync(string id) => throw new NotImplementedException();

    public Task<IEnumerable<Users>> FindAsync(Expression<Func<Users, bool>> filter) => throw new NotImplementedException();

    public Task<IEnumerable<Users>> GetAllAsync() => throw new NotImplementedException();

    public Task<Users> GetByIdAsync(string id) => throw new NotImplementedException();

    public Task UpdateAsync(Users entity) => throw new NotImplementedException();
}
