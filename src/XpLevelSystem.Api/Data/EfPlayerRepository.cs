using Microsoft.EntityFrameworkCore;
using XpLevelSystem.Core;

namespace XpLevelSystem.Api.Data;

public class EfPlayerRepository : IPlayerRepository
{
    private readonly AppDbContext _context;

    public EfPlayerRepository(AppDbContext context)
    {
        _context = context;
    }

    public int Add(Player player)
    {
        var entry = _context.Players.Add(player);
        _context.SaveChanges();
        return entry.Property<int>("Id").CurrentValue;
    }

    public Player? GetById(int id)
    {
        return _context.Players.Find(id);
    }

    public void Update(int id, Player player)
    {
        var entry = _context.Entry(player);

        if (entry.State == EntityState.Detached)
        {
            _context.Players.Attach(player);
            entry = _context.Entry(player);
            entry.Property("Id").CurrentValue = id;
        }

        entry.State = EntityState.Modified;
        _context.SaveChanges();
    }
}
