using MongoDB.Driver;

using Nastaran_bot.Repositories.DailyNote;
using Nastaran_bot.Repositories.Idea;
using Nastaran_bot.Repositories.Inspiration;
using Nastaran_bot.Repositories.User;
using Nastaran_bot.Services.DailyNote;
using Nastaran_bot.Services.Idea;
using Nastaran_bot.Services.Inspiration;
using Nastaran_bot.Services.TelegramBot;
using Nastaran_bot.Services.User;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ========================
// 1. Add Controllers
// ========================
// This allows your application to use API controllers for handling HTTP requests.
// Telegram webhook updates will be sent to a controller endpoint.
builder.Services.AddControllers();

// ========================
// 2️. Swagger/OpenAPI
// ========================
// Adds Swagger UI for testing your API endpoints in development.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================
// 3️. MongoDB Client
// ========================
// Register a single MongoClient as a singleton.
// It will be shared across all repositories.
// Ensure you have the connection string in appsettings.Development.json:
// "MongoDb": "mongodb+srv://<username>:<password>@cluster0.mongodb.net"
builder.Services.AddSingleton<IMongoClient>(s =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDb")));

// ========================
// 4️. Repositories DI
// ========================
// Add Scoped repositories for dependency injection.
// Each repository gets IMongoClient injected automatically.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDailyNoteRepository, DailyNoteRepository>();
builder.Services.AddScoped<IIdeaRepository, IdeaRepository>();
builder.Services.AddScoped<IInspirationRepository, InspirationRepository>();

// ========================
// 5️. Services DI
// ========================
// Add Scoped services. They will use the repositories internally.
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDailyNoteService, DailyNoteService>();
builder.Services.AddScoped<IIdeaService, IdeaService>();
builder.Services.AddScoped<IInspirationService, InspirationService>();

// ========================
// 6️. TelegramBotService DI
// ========================
// The orchestrator that handles updates, calling other services.
builder.Services.AddScoped<TelegramBotService>();

// ========================
// 7️. Build the app
// ========================
WebApplication app = builder.Build();

// ========================
// 8️. Middleware
// ========================

// Enable Swagger UI only in development environment
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    // Enable HSTS for production
    _ = app.UseHsts();
    // Enable HTTPS redirection for production
    _ = app.UseHttpsRedirection();
}

// Configure the app to listen on all network interfaces at port 8080
app.Urls.Add("http://0.0.0.0:8080");

// Enable Authentication middleware (if you add authentication later)
app.UseAuthentication();

// Enable Authorization middleware (if you add policies later)
app.UseAuthorization();

// Map controller endpoints (e.g., Telegram webhook)
app.MapControllers();

// ========================
// 9️. Run the app
// ========================
app.Run();
