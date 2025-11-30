using Microsoft.AspNetCore.Mvc;

using Nastaran_bot.Services.TelegramBot;

namespace Nastaran_bot.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TelegramController(TelegramBotService telegramBotService) : ControllerBase
{
    private readonly TelegramBotService _telegramBotService = telegramBotService;
}
