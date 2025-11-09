using Nastaran_bot.Services.DailyNote;
using Nastaran_bot.Services.Idea;
using Nastaran_bot.Services.Inspiration;
using Nastaran_bot.Services.User;

namespace Nastaran_bot.Services.TelegramBot;

public class TelegramBotService(
    IUserService userService,
    IDailyNoteService dailyNoteService,
    IIdeaService ideaService,
    IInspirationService inspirationService)
{
    private readonly IUserService _userService = userService;
    private readonly IDailyNoteService _dailyNoteService = dailyNoteService;
    private readonly IIdeaService _ideaService = ideaService;
    private readonly IInspirationService _inspirationService = inspirationService;
}
