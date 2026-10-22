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

    public Player? GetById(int id)
    {
        _players.TryGetValue(id, out Player? player);
        return player;
    }

    public void Update(int id, Player player)
    {
        _players[id] = player;
    }
}