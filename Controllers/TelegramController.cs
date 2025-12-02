using Microsoft.AspNetCore.Mvc;

using Nastaran_bot.Services.TelegramBot;

namespace Nastaran_bot.Controllers;

/// <summary>
/// API endpoints used for handling incoming Telegram webhook requests
/// and delegating message processing to the bot service.
/// </summary>
/// <param name="telegramBotService">
/// The service responsible for processing Telegram bot updates and commands.
/// </param>
[ApiController]
[Route("api/[controller]/[action]")]
public class TelegramController(TelegramBotService telegramBotService) : ControllerBase
{
    private readonly TelegramBotService _telegramBotService = telegramBotService;
}
