
namespace XpLevelSystem.Core;

public class FakePlayerRepository : IPlayerRepository
{
    private readonly Dictionary<int, Player> _players = new();
    private int _nextId = 1;

    public int Add(Player player)
    {
        int id = _nextId++;
        _players.Add(id, player);
        return id;
    }

    public Player? GetById(Guid userId, int id)
    {
        if (_players.TryGetValue(id, out var player)
            && player.UserId == userId)
        {
            return player;
        }

        return null;
    }

    public void Update(Guid userId, int id, Player player)
    {
        if (_players.TryGetValue(id, out var existingPlayer)
            && existingPlayer.UserId == userId
            && player.UserId == userId)
        {
            _players[id] = player;
        }
    }

    public bool Delete(Guid userId, int id)
    {
        var player = GetById(userId, id);

        if (player == null)
        {
            return false;
        }

        return _players.Remove(id);
    }
}
