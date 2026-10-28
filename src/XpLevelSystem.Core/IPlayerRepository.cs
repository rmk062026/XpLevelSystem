namespace XpLevelSystem.Core;

public interface IPlayerRepository
{
    int Add(Player player);

    Player? GetById(Guid userId, int id);

    void Update(Guid userId, int id, Player player);
}