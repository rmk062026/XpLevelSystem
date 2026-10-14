using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace XpLevelSystem.Api.Tests;

public class PlayerApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PlayerApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePlayer_ShouldReturn201Created()
    {
        // Arrange
        var request = new { Name = "Martin" };

        // Act
        var response = await _client.PostAsJsonAsync("/api/players", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetPlayer_ShouldReturn200WhenPlayerExists()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync(
            "/api/players",
            new { Name = "Martin" });

        var createdPlayer = await createResponse.Content
            .ReadFromJsonAsync<PlayerResponse>();

        Assert.NotNull(createdPlayer);

        // Act
        var response = await _client.GetAsync(
            $"/api/players/{createdPlayer.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var player = await response.Content
            .ReadFromJsonAsync<PlayerResponse>();

        Assert.NotNull(player);
        Assert.Equal("Martin", player.Name);
        Assert.Equal(0, player.Experience);
        Assert.Equal(1, player.Level);
    }

    [Fact]
    public async Task GetPlayer_ShouldReturn404WhenPlayerDoesNotExist()
    {
        // Act
        var response = await _client.GetAsync("/api/players/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddExperience_ShouldUpdatePlayerXpAndLevel()
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync(
            "/api/players",
            new { Name = "Martin" });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdPlayer = await createResponse.Content
            .ReadFromJsonAsync<PlayerResponse>();

        Assert.NotNull(createdPlayer);

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/players/{createdPlayer.Id}/experience",
            new { Amount = 250 });

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var player = await response.Content
            .ReadFromJsonAsync<PlayerResponse>();

        Assert.NotNull(player);
        Assert.Equal(250, player.Experience);
        Assert.Equal(3, player.Level);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-50)]
    public async Task AddExperience_ShouldReturn400ForInvalidXp(int amount)
    {
        // Arrange
        var createResponse = await _client.PostAsJsonAsync(
            "/api/players",
            new { Name = "Martin" });

        var createdPlayer = await createResponse.Content
            .ReadFromJsonAsync<PlayerResponse>();

        Assert.NotNull(createdPlayer);

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/players/{createdPlayer.Id}/experience",
            new { Amount = amount });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        // Kontroller at spilleren ikke ble endret
        var getResponse = await _client.GetAsync(
            $"/api/players/{createdPlayer.Id}");

        var player = await getResponse.Content
            .ReadFromJsonAsync<PlayerResponse>();

        Assert.NotNull(player);
        Assert.Equal(0, player.Experience);
        Assert.Equal(1, player.Level);
    }
}
public record PlayerResponse(
    int Id,
    string Name,
    int Experience,
    int Level);