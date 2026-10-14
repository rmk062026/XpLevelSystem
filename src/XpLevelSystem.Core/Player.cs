namespace XpLevelSystem.Core;

public class Player
{
    public string Name { get; set; }
    public int Experience { get; private set; }
    public int Level { get; private set; }

    public Player(string name)
    {
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