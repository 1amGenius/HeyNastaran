
using MongoDB.Driver;

using Nastaran_bot.Repositories.Quote;
using Nastaran_bot.Repositories.Idea;
using Nastaran_bot.Repositories.Inspiration;
using Nastaran_bot.Repositories.User;
using Nastaran_bot.Services.Quote;
using Nastaran_bot.Services.Idea;
using Nastaran_bot.Services.Inspiration;
using Nastaran_bot.Services.TelegramBot;
using Nastaran_bot.Services.TelegramBot.Handlers.Commands;
using Nastaran_bot.Services.TelegramBot.Handlers.Updates;
using Nastaran_bot.Services.TelegramBot.Interfaces;
using Nastaran_bot.Services.TelegramBot.Routing;
using Nastaran_bot.Services.User;
using Nastaran_bot.Utils.Helpers.Music.Clients;
using Nastaran_bot.Utils.Helpers.Music.Interfaces;
using Nastaran_bot.Utils.Helpers.Scheduler.Clients;
using Nastaran_bot.Utils.Helpers.Scheduler.Interfaces;
using Nastaran_bot.Utils.Helpers.Weather.Clients;
using Nastaran_bot.Utils.Helpers.Weather.Interfaces;

using Telegram.Bot;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ========================
// 1. Add Controllers
// ========================
builder.Services.AddControllers();

// ========================
// 2️. Swagger/OpenAPI
// ========================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================
// 3️. MongoDB Client
// ========================
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

// ========================
// 4. TelegramBot Client
// ========================
builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    IConfiguration config = sp.GetRequiredService<IConfiguration>();
    string token = config["Telegram:BotToken"];
    return new TelegramBotClient(token);
});

// ========================
// 5. Weather Clients
// ========================
// 1. Register the HTTP client (transient under the hood, created via factory)
builder.Services.AddHttpClient<IWeatherHttpClient, WeatherHttpClient>(client => client.Timeout = TimeSpan.FromSeconds(10));

// 2. Register WeatherApiClient as Scoped (can now receive IWeatherHttpClient)
builder.Services.AddScoped<IWeatherApiClient, WeatherApiClient>();

// ========================
// 6. Repositories DI
// ========================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IQuoteRepository, QuoteRepository>();
builder.Services.AddScoped<IIdeaRepository, IdeaRepository>();
builder.Services.AddScoped<IInspirationRepository, InspirationRepository>();

// ========================
// 7. Services DI
// ========================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQuoteService, QuoteService>();
builder.Services.AddScoped<IIdeaService, IdeaService>();
builder.Services.AddScoped<IInspirationService, InspirationService>();

// COMMAND handlers
builder.Services.AddScoped<ICommandHandler, NoteCommandHandler>();
builder.Services.AddScoped<ICommandHandler, IdeaCommandHandler>();
builder.Services.AddScoped<ICommandHandler, InspirationCommandHandler>();
builder.Services.AddScoped<ICommandHandler, WeatherCommandHandler>();
builder.Services.AddScoped<ICommandHandler, StartCommandHandler>();

// UPDATE handlers
builder.Services.AddScoped<IUpdateHandler, WeatherCityHandler>();
builder.Services.AddScoped<IUpdateHandler, WeatherLocationHandler>();

// Routers
builder.Services.AddScoped<CommandRouter>();
builder.Services.AddScoped<UpdateRouter>();

// ========================
// 8. TelegramBotService DI
// ========================
builder.Services.AddScoped<TelegramBotService>();

// ========================
// 9. Helpers DI
// ========================
// These helpers do not require HttpClient injection,
// so they can be safely registered as Singletons.
builder.Services.AddSingleton<IMusicApiClient, MusicApiClient>();
builder.Services.AddSingleton<IScheduler, Scheduler>();

// ========================
// 10. Build the app
// ========================
WebApplication app = builder.Build();

// ========================
// 11. Middleware
// ========================
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    _ = app.UseHsts();
    _ = app.UseHttpsRedirection();
}

app.Urls.Add("http://0.0.0.0:8080");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ========================
// 12. Run the app
// ========================
app.Run();
