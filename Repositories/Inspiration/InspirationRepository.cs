using System.Linq.Expressions;

using Nastaran_bot.Models;

namespace Nastaran_bot.Repositories.Inspiration
{
    public class InspirationRepository : IInspirationRepository
    {
        public Task CreateAsync(Inspirations entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Inspirations>> FindAsync(Expression<Func<Inspirations, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Inspirations>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Inspirations> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Inspirations entity)
        {
            throw new NotImplementedException();
        }
    }
}
