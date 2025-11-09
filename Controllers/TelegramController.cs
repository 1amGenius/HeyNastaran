using Microsoft.AspNetCore.Mvc;

using Nastaran_bot.Services.TelegramBot;

namespace Nastaran_bot.Controllers;

public class TelegramController(TelegramBotService telegramBotService) : ControllerBase
{
    private readonly TelegramBotService _telegramBotService = telegramBotService;
}
