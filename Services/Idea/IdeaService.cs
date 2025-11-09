using Nastaran_bot.Repositories.Idea;

namespace Nastaran_bot.Services.Idea;

public class IdeaService(IIdeaRepository ideaRepository) : IIdeaService
{
    private readonly IIdeaRepository _ideaRepository = ideaRepository;
}
