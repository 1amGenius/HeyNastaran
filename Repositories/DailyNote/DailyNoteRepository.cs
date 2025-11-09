using System.Linq.Expressions;

using Nastaran_bot.Models;

namespace Nastaran_bot.Repositories.DailyNote;

public class DailyNoteRepository : IDailyNoteRepository
{
    public Task CreateAsync(DailyNotes entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DailyNotes>> FindAsync(Expression<Func<DailyNotes, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<DailyNotes>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<DailyNotes> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(DailyNotes entity)
    {
        throw new NotImplementedException();
    }
}
