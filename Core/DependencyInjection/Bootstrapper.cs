using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Telegram.Bot;

using Core.Repositories.Idea;
using Core.Repositories.Inspiration;
using Core.Repositories.Quote;
using Core.Repositories.User;
using Core.Services.Idea;
using Core.Services.Inspiration;
using Core.Services.Quote;
using Core.Services.TelegramBot;
using Core.Services.TelegramBot.Handlers.Commands;
using Core.Services.TelegramBot.Handlers.Commands.Idea;
using Core.Services.TelegramBot.Handlers.Commands.Inspiration;
using Core.Services.TelegramBot.Handlers.Commands.Note;
using Core.Services.TelegramBot.Handlers.Commands.Weather;
using Core.Services.TelegramBot.Handlers.Updates.Inspiration;
using Core.Services.TelegramBot.Handlers.Updates.Weather;
using Core.Services.TelegramBot.Interfaces;
using Core.Services.TelegramBot.Routing;
using Core.Services.TelegramBot.State.Inspiration;
using Core.Services.User;
using Core.Utils.Helpers.Music.Clients;
using Core.Utils.Helpers.Music.Interfaces;
using Core.Utils.Helpers.Scheduler.Clients;
using Core.Utils.Helpers.Scheduler.Interfaces;
using Core.Utils.Helpers.Weather.Clients;
using Core.Utils.Helpers.Weather.Interfaces;

namespace Core.DependencyInjection;

public static class Bootstrapper
{
    public static IServiceCollection AddBotApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ========================
        // MongoDB
        // ========================
        _ = services.AddSingleton<IMongoClient>(_ =>
            new MongoClient(configuration.GetConnectionString("MongoDb")));

        // ========================
        // Telegram Bot Client
        // ========================
        _ = services.AddSingleton<ITelegramBotClient>(_ =>
            new TelegramBotClient(
                configuration.GetConnectionString("BotToken")!
            ));

        // ========================
        // Weather HTTP + API
        // ========================
        _ = services.AddHttpClient<IWeatherHttpClient, WeatherHttpClient>(client =>
            client.Timeout = TimeSpan.FromSeconds(10));

        _ = services.AddScoped<IWeatherApiClient, WeatherApiClient>();

        // ========================
        // Repositories
        // ========================
        _ = services.AddScoped<IUserRepository, UserRepository>();
        _ = services.AddScoped<IQuoteRepository, QuoteRepository>();
        _ = services.AddScoped<IIdeaRepository, IdeaRepository>();
        _ = services.AddScoped<IInspirationRepository, InspirationRepository>();

        // ========================
        // Domain Services
        // ========================
        _ = services.AddScoped<IUserService, UserService>();
        _ = services.AddScoped<IQuoteService, QuoteService>();
        _ = services.AddScoped<IIdeaService, IdeaService>();
        _ = services.AddScoped<IInspirationService, InspirationService>();

        // ========================
        // Stores (stateful)
        // ========================
        _ = services.AddSingleton<InspirationEditStore>();
        _ = services.AddSingleton<InspirationCreateIntentStore>();

        // ========================
        // Command Handlers
        // ========================
        _ = services.AddScoped<ICommandHandler, NoteCommandHandler>();
        _ = services.AddScoped<ICommandHandler, IdeaCommandHandler>();
        _ = services.AddScoped<ICommandHandler, InspirationCommandHandler>();
        _ = services.AddScoped<ICommandHandler, WeatherCommandHandler>();
        _ = services.AddScoped<ICommandHandler, StartCommandHandler>();

        // ========================
        // Update Handlers
        // ========================
        _ = services.AddScoped<IUpdateHandler, WeatherCallbackHandler>();
        _ = services.AddScoped<IUpdateHandler, WeatherCityHandler>();
        _ = services.AddScoped<IUpdateHandler, WeatherLocationHandler>();
        _ = services.AddScoped<IUpdateHandler, WeatherSearchCityHandler>();

        _ = services.AddScoped<IUpdateHandler, InspirationCallbackHandler>();
        _ = services.AddScoped<IUpdateHandler, InspirationCreateHandler>();
        _ = services.AddScoped<IUpdateHandler, InspirationEditHandler>();

        // ========================
        // Routers
        // ========================
        _ = services.AddScoped<CommandRouter>();
        _ = services.AddScoped<UpdateRouter>();

        // ========================
        // Telegram Bot Service
        // ========================
        _ = services.AddScoped<TelegramBotService>();

        // ========================
        // Helpers
        // ========================
        _ = services.AddSingleton<IMusicApiClient, MusicApiClient>();
        _ = services.AddSingleton<IScheduler, Scheduler>();

        return services;
    }
}

