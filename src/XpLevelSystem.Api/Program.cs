using Microsoft.EntityFrameworkCore;
using XpLevelSystem.Api.Data;
using XpLevelSystem.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");

if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<AppDbContext>(
        options => options.UseNpgsql(connectionString));

    builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
}
else if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSingleton<IPlayerRepository, FakePlayerRepository>();
}
else
{
    throw new InvalidOperationException(
        "Database connection string is missing.");
}

builder.Services.AddScoped<PlayerService>();

var app = builder.Build();

if (!string.IsNullOrWhiteSpace(connectionString))
{
    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    database.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program { }
