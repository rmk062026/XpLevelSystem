using Xunit;

namespace XpLevelSystem.Core.Tests;

public class PlayerTests
{
    [Fact]
    public void NewPlayer_ShouldStartWithZeroXpAndLevelOne()
    {
        // Arrange
        var player = new Player("Martin");

        // Act

        // Assert
        Assert.Equal(0, player.Experience);
        Assert.Equal(1, player.Level);
    }

    [Fact]
    public void AddExperience_ShouldIncreaseXpWithoutLevelUp()
    {
        // Arrange
        var player = new Player("Martin");

        // Act
        player.AddExperience(50);

        // Assert
        Assert.Equal(50, player.Experience);
        Assert.Equal(1, player.Level);
    }

    [Fact]
    public void AddExperience_ShouldIncreaseLevel()
    {
        // Arrange
        var player = new Player("Martin");

        // Act
        player.AddExperience(100);

        // Assert
        Assert.Equal(2, player.Level);
    }
}