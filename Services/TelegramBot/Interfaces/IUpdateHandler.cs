using Telegram.Bot.Types;

namespace Nastaran_bot.Services.TelegramBot.Interfaces;

public interface IUpdateHandler
{
    public bool CanHandle(Update update);

    public Task HandleAsync(Update update);
}
