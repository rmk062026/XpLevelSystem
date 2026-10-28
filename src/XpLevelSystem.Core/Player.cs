namespace XpLevelSystem.Core;

public class Player
{
    public Guid UserId { get; private set; }
    public string Name { get; set; }
    public int Experience { get; private set; }
    public int Level { get; private set; }

    public Player(Guid userId, string name)
    {
        UserId = userId;
        Name = name;
        Experience = 0;
        Level = 1;
    }

    public void AddExperience(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount));
        }
        Experience += amount;
        Level = 1 + (Experience / 100);
    }
}