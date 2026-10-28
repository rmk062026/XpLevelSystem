namespace XpLevelSystem.Core;

public class PlayerService
{
    private readonly IPlayerRepository _repository;

    public PlayerService(IPlayerRepository repository)
    {
        _repository = repository;
    }

    public int CreatePlayer(Guid userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Spillernavn kan ikke være tomt.");
        }

        var player = new Player(userId, name);
        return _repository.Add(player);
    }

    public Player? GetPlayer(Guid userId, int id)
    {
        var player = _repository.GetById(id);

        if (player == null || player.UserId != userId)
        {
            return null;
        }

        return player;
    }

    public Player? AddExperience(Guid userId, int id, int amount)
    {
        var player = GetPlayer(userId, id);

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