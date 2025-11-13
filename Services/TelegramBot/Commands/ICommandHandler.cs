using Telegram.Bot.Types;

namespace Nastaran_bot.Services.TelegramBot.Commands;

public interface ICommandHandler
{
    public string Command
    {
        get;
    }

    public Task HandleAsync(Update update);
}

