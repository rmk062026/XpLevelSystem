namespace XpLevelSystem.Core;

public interface IPlayerRepository
{
    int Add(Player player);

    Player? GetById(int id);

    void Update(int id, Player player);
}