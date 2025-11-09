using Nastaran_bot.Repositories.DailyNote;

namespace Nastaran_bot.Services.DailyNote;

public class DailyNoteService(IDailyNoteRepository dailyNoteRepository) : IDailyNoteService
{
    private readonly IDailyNoteRepository _dailyNoteRepository = dailyNoteRepository;
}
