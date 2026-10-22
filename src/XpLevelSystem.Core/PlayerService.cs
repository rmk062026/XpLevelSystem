namespace XpLevelSystem.Core;

public class PlayerService
{
    private readonly IPlayerRepository _repository;

    public PlayerService(IPlayerRepository repository)
    {
        _repository = repository;
    }

    public int CreatePlayer(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Spillernavn kan ikke være tomt.");
        }

        var player = new Player(name);
        return _repository.Add(player);
    }

    public Player? GetPlayer(int id)
    {
        return _repository.GetById(id);
    }

    public Player? AddExperience(int id, int amount)
    {
        var player = _repository.GetById(id);

        if (player == null)
        {
            return null;
        }

        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }

        player.AddExperience(amount);
        _repository.Update(id, player);

        return player;
    }
}