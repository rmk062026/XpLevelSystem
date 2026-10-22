using Microsoft.EntityFrameworkCore;
using XpLevelSystem.Core;

namespace XpLevelSystem.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Player> Players => Set<Player>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Player>(entity =>
        {
            entity.ToTable("Players");
            entity.Property<int>("Id").ValueGeneratedOnAdd();
            entity.HasKey("Id");
            entity.Property(p => p.Name).IsRequired();
            entity.Property(p => p.Experience);
            entity.Property(p => p.Level);
        });
    }
}
