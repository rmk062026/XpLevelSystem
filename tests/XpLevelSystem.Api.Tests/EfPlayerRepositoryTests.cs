
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using XpLevelSystem.Api.Data;
using XpLevelSystem.Core;

namespace XpLevelSystem.Api.Tests;

public class EfPlayerRepositoryTests
{
    [Fact]
    public void Update_SavesExperienceAndLevel()
    {
        // SQLite-databasen finnes bare mens testen kjører.
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var userId = Guid.NewGuid();
        int playerId;

        using (var context = new AppDbContext(options))
        {
            context.Database.EnsureCreated();

            var repository = new EfPlayerRepository(context);
            playerId = repository.Add(new Player(userId, "Testspiller"));

            var service = new PlayerService(repository);
            service.AddExperience(userId, playerId, 150);
        }

        // Ny DbContext viser at verdiene faktisk ble lagret.
        using (var context = new AppDbContext(options))
        {
            var repository = new EfPlayerRepository(context);
            var player = repository.GetById(userId, playerId);

            Assert.NotNull(player);
            Assert.Equal(150, player.Experience);
            Assert.Equal(2, player.Level);
        }
    }

    [Fact]
    public void Update_RejectsPlayerThatIsNotTracked()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        var repository = new EfPlayerRepository(context);
        var userId = Guid.NewGuid();

        var playerId = repository.Add(new Player(userId, "Testspiller"));

        // Dette er et nytt objekt, ikke spilleren EF Core sporer.
        var otherPlayer = new Player(userId, "Annen spiller");

        Assert.Throws<InvalidOperationException>(() =>
            repository.Update(userId, playerId, otherPlayer));
    }
}
