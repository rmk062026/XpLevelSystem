using Xunit;

namespace XpLevelSystem.Core.Tests;

public class PlayerTests
{
    [Fact]
    public void NewPlayer_ShouldStartWithZeroXpAndLevelOne()
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), "Martin");
        // Act

        // Assert
        Assert.Equal(0, player.Experience);
        Assert.Equal(1, player.Level);
    }

    [Fact]
    public void AddExperience_ShouldIncreaseXpWithoutLevelUp()
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), "Martin");
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
        var player = new Player(Guid.NewGuid(), "Martin");
        // Act
        player.AddExperience(100);

        // Assert
        Assert.Equal(2, player.Level);
    }

    [Fact]
    public void AddExperience_ShouldIncreaseMultipleLevels()
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), "Martin");
        // Act
        player.AddExperience(250);

        // Assert
        Assert.Equal(250, player.Experience);
        Assert.Equal(3, player.Level);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public void AddExperience_ShouldRejectInvalidAmount(int amount)
    {
        // Arrange
        var player = new Player(Guid.NewGuid(), "Martin");
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(
            () => player.AddExperience(amount)
        );

        Assert.Equal(0, player.Experience);
        Assert.Equal(1, player.Level);
    }
}