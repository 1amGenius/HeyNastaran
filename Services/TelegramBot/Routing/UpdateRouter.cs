using Nastaran_bot.Services.TelegramBot.Interfaces;

using Telegram.Bot.Types;

namespace Nastaran_bot.Services.TelegramBot.Routing;

public class UpdateRouter(IEnumerable<IUpdateHandler> handlers)
{
    private readonly IEnumerable<IUpdateHandler> _handlers = handlers;

    public async Task RouteAsync(Update update)
    {
        foreach (IUpdateHandler handler in _handlers)
        {
            if (handler.CanHandle(update))
            {
                await handler.HandleAsync(update);
                return;
            }
        }
    }
}
