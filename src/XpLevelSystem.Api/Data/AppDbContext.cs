
using Microsoft.EntityFrameworkCore;
using XpLevelSystem.Core;

namespace XpLevelSystem.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Player> Players => Set<Player>();
    public DbSet<User> Users => Set<User>();

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

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();
        });
    }
}
