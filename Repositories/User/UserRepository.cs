using System.Linq.Expressions;

using Nastaran_bot.Models;

namespace Nastaran_bot.Repositories.User;

public class UserRepository : IUserRepository
{
    public Task CreateAsync(Users entity) => throw new NotImplementedException();

    public Task DeleteAsync(string id) => throw new NotImplementedException();

    public Task<IEnumerable<Users>> FindAsync(Expression<Func<Users, bool>> filter) => throw new NotImplementedException();

    public Task<IEnumerable<Users>> GetAllAsync() => throw new NotImplementedException();

    public Task<Users> GetByIdAsync(string id) => throw new NotImplementedException();

    public Task UpdateAsync(Users entity) => throw new NotImplementedException();
}
