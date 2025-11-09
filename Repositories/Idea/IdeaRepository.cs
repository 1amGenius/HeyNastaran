using System.Linq.Expressions;

using MongoDB.Driver;

using Nastaran_bot.Models;

namespace Nastaran_bot.Repositories.Idea;

public class IdeaRepository : IIdeaRepository
{
    private readonly IMongoCollection<Ideas> _ideas;

    public IdeaRepository(IMongoClient client)
    {
        IMongoDatabase database = client.GetDatabase("nastaranBotDb");
        _ideas = database.GetCollection<Ideas>("ideas");
    }

    public Task CreateAsync(Ideas entity) => throw new NotImplementedException();

    public Task DeleteAsync(string id) => throw new NotImplementedException();

    public Task<IEnumerable<Ideas>> FindAsync(Expression<Func<Ideas, bool>> filter) => throw new NotImplementedException();

    public Task<IEnumerable<Ideas>> GetAllAsync() => throw new NotImplementedException();

    public Task<Ideas> GetByIdAsync(string id) => throw new NotImplementedException();

    public Task UpdateAsync(Ideas entity) => throw new NotImplementedException();
}
