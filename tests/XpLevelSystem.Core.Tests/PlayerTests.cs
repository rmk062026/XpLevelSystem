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
}