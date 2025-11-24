using System.Linq.Expressions;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Nastaran_bot.Repositories.DailyNote;

public class DailyNoteRepository : IDailyNoteRepository
{
    private readonly IMongoCollection<Models.DailyNote> _dailyNotes;

    public DailyNoteRepository(IMongoClient client)
    {
        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _dailyNotes = database.GetCollection<Models.DailyNote>("dailyNotes");
    }

    public async Task CreateAsync(Models.DailyNote entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        await _dailyNotes.InsertOneAsync(entity);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return false;
        }

        DeleteResult result = await _dailyNotes.DeleteOneAsync(n => n.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<IEnumerable<Models.DailyNote>> FindAsync(Expression<Func<Models.DailyNote, bool>> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return await _dailyNotes.AsQueryable().Where(filter).ToListAsync();
    }

    public async Task<IEnumerable<Models.DailyNote>> FindAllAsync()
        => await _dailyNotes.Find(_ => true).ToListAsync();

    public async Task<Models.DailyNote> FindByIdAsync(string id)
    {
        FilterDefinition<Models.DailyNote> filter = Builders<Models.DailyNote>.Filter.Eq(n => n.Id, id);
        return await _dailyNotes.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Models.DailyNote>> FindByTelegramIdAsync(long telegramId)
    {
        FilterDefinition<Models.DailyNote> filter = Builders<Models.DailyNote>.Filter.Eq("telegramId", telegramId);
        return await _dailyNotes.Find(filter).ToListAsync();
    }

    public async Task UpdateAsync(Models.DailyNote entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        ArgumentException.ThrowIfNullOrEmpty(entity.Id, nameof(entity.Id));

        entity.UpdatedAt = DateTime.UtcNow;

        FilterDefinition<Models.DailyNote> filter = Builders<Models.DailyNote>.Filter.Eq(n => n.Id, entity.Id);
        ReplaceOneResult result = await _dailyNotes.ReplaceOneAsync(filter, entity);

        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"DailyNote with Id {entity.Id} was not found.");
        }
    }
}
