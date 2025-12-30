using Core.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// ========================
// ASP.NET-only concerns
// ========================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ========================
// Application wiring (Core)
// ========================
builder.Services.AddBotApplication(builder.Configuration);

// ========================
WebApplication app = builder.Build();
// ========================

if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}
else
{
    _ = app.UseHsts();
    _ = app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
