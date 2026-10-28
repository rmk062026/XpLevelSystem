
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

    public Player? GetById(Guid userId, int id)
    {
        return _context.Players.FirstOrDefault(p =>
            EF.Property<int>(p, "Id") == id &&
            p.UserId == userId);
    }

    public void Update(Guid userId, int id, Player player)
    {
        var existingPlayer = GetById(userId, id);

        if (existingPlayer == null || player.UserId != userId)
        {
            return;
        }

        existingPlayer.Name = player.Name;

        _context.SaveChanges();
    }

    public bool Delete(Guid userId, int id)
    {
        var player = GetById(userId, id);

        if (player == null)
        {
            return false;
        }

        _context.Players.Remove(player);
        _context.SaveChanges();

        return true;
    }
}
