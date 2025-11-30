using Nastaran_bot.Contracts.User;
using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Services.User;
using Nastaran_bot.Utils;
using Nastaran_bot.Utils.Mappers;

using Telegram.Bot;
using Telegram.Bot.Types;

namespace Nastaran_bot.Services.TelegramBot.Handlers.Updates.Weather;

public class WeatherSearchStartHandler(
    ITelegramBotClient botClient,
    IUserService userService
) : IUpdateHandler
{
    private readonly ITelegramBotClient _botClient = botClient;
    private readonly IUserService _userService = userService;

    public bool CanHandle(Update update)
        => update.Message?.Text == BotButtons.Texts.Weather.SearchCity;

    public async Task HandleAsync(Update update)
    {
        long chatId = update.Message!.Chat.Id;
        long telegramId = update.Message.From.Id;

        Models.User user = await _userService.GetUserByTelegramIdAsync(telegramId);
        if (user != null)
        {
            UserUpdateDto userDto = UserMapper.ToUpdateDto(user);
            userDto.IsSearchingCity = true;
            _ = await _userService.UpdateUserAsync(user.Id, userDto);
        }

        _ = await _botClient.SendMessage(
            chatId,
            "Send me the city name you want to check 🌤:"
        );
    }
}
