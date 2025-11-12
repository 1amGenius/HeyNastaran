using System.Linq.Expressions;

namespace Nastaran_bot.Repositories;

public interface IRepository<T> where T : class
{
    public Task<T> GetByIdAsync(string id);
    public Task<IEnumerable<T>> GetAllAsync();
    public Task<IEnumerable<T>> GetByTelegramIdAsync(long telegramId);
    public Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> filter);
    public Task CreateAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task<bool> DeleteAsync(string id);
}
