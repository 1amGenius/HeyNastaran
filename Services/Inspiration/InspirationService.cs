using Nastaran_bot.Repositories.Inspiration;

namespace Nastaran_bot.Services.Inspiration;

public class InspirationService(IInspirationRepository inspirationRepository) : IInspirationService
{
    private readonly IInspirationRepository _inspirationRepository = inspirationRepository;
}
