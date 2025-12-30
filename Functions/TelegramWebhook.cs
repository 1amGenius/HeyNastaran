using System.Net;
using System.Text.Json;

using Core.Services.TelegramBot;

using Microsoft.Extensions.DependencyInjection;

using Telegram.Bot.Types;

namespace Functions;

public static class TelegramWebhook
{
    public static async Task<HttpResponseMessage> Run(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Content is null)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        string body = await request.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(body))
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        Update update;
        try
        {
            update = JsonSerializer.Deserialize<Update>(body);
        }
        catch
        {
            // Telegram retries on non-200; invalid JSON should still be rejected
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        if (update is null)
        {
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        TelegramBotService botService = FunctionBootstrapper.ServiceProvider.GetRequiredService<TelegramBotService>();

        await botService.HandleUpdateAsync(update, cancellationToken);

        return new HttpResponseMessage(HttpStatusCode.OK);
    }
}
