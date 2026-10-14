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
}