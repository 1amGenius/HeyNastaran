using Nastaran_bot.Contracts.User;
using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Services.User;
using Nastaran_bot.Utils.Formaters;
using Nastaran_bot.Utils.Helpers.Weather.Interfaces;
using Nastaran_bot.Utils.Mappers;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Updates.Weather;

public class WeatherSearchCityHandler(
    ITelegramBotClient botClient,
    IWeatherApiClient weatherApi,
    IUserService userService,
    ILogger<WeatherSearchCityHandler> logger
) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IWeatherApiClient _weatherApi = weatherApi;
    private readonly IUserService _userService = userService;
    private readonly ILogger<WeatherSearchCityHandler> _logger = logger;

    public bool CanHandle(Update update)
    {
        if (update.Message?.Text == null)
        {
            return false;
        }

        Models.User user = _userService.GetUserByTelegramIdAsync(update.Message.From.Id).Result;
        return user != null && user.IsSearchingCity && update.Message.Text.Length >= 2;
    }

    public async Task HandleAsync(Update update)
    {
        long chatId = update.Message!.Chat.Id;
        long telegramId = update.Message.From.Id;
        string city = update.Message.Text.Trim();

        try
        {
            (float lat, float lon) = await _weatherApi.GetCoordinatesByCityNameAsync(city);
            Models.Weather weather = await _weatherApi.GetCurrentWeatherAsync(lat, lon);

            _ = await _botClient.SendMessage(
                chatId,
                FormatWeather.CitySummary(city, weather.Current),
                parseMode: ParseMode.MarkdownV2
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch weather for city '{City}'", city);

            _ = await _botClient.SendMessage(
                chatId,
                "⚠️ I couldn't find weather information for that city. Try something like: `London`",
                parseMode: ParseMode.MarkdownV2
            );
        }
        finally
        {
            Models.User user = await _userService.GetUserByTelegramIdAsync(telegramId);
            if (user != null)
            {
                UserUpdateDto userDto = UserMapper.ToUpdateDto(user);
                userDto.IsSearchingCity = true;
                _ = await _userService.UpdateUserAsync(user.Id, userDto);
            }
        }
    }
}
