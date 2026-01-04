using System.Net;
using System.Text.Json;

using Core.Services.TelegramBot;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

using Telegram.Bot.Types;

namespace Function;

public class TelegramWebhook(TelegramBotService botService)
{
    private readonly TelegramBotService _botService = botService;

    [Function("TelegramWebhook")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
        HttpRequestData request,
        FunctionContext context)
    {
        string body = await new StreamReader(request.Body).ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(body))
        {
            return request.CreateResponse(HttpStatusCode.BadRequest);
        }

        Update update;
        try
        {
            update = JsonSerializer.Deserialize<Update>(body);
        }
        catch
        {
            return request.CreateResponse(HttpStatusCode.BadRequest);
        }

        if (update is null)
        {
            return request.CreateResponse(HttpStatusCode.BadRequest);
        }

        await _botService.HandleUpdateAsync(update, context.CancellationToken);

        return request.CreateResponse(HttpStatusCode.OK);
    }
}
