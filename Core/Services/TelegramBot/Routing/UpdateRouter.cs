using Core.Services.TelegramBot.Interfaces;

using Telegram.Bot.Types;

namespace Core.Services.TelegramBot.Routing;

/// <summary>
/// Routes non-command updates to the first compatible <see cref="IUpdateHandler"/>.
/// </summary>
/// <remarks>
/// The router evaluates handlers in registration order.
/// Once a handler accepts an update, routing stops.
/// </remarks>
public sealed class UpdateRouter(IEnumerable<IUpdateHandler> handlers)
{
    private readonly IEnumerable<IUpdateHandler> _handlers = handlers;

    /// <summary>
    /// Routes an update to the first handler that can process it.
    /// </summary>
    /// <param name="update">The Telegram update to route.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    public async Task RouteAsync(Update update, CancellationToken cancellationToken = default)
    {
        foreach (IUpdateHandler handler in _handlers)
        {
            if (handler.CanHandle(update))
            {
                await handler.HandleAsync(update, cancellationToken);
                return;
            }
        }
    }
}
