using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace XpLevelSystem.Api.Tests;

public class PlayerApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;


    public PlayerApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        var configuration = factory.Services
            .GetRequiredService<IConfiguration>();

        var jwtKey = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        var userId = Guid.NewGuid();

        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Name, "TestUser")
    };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokenString);
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

    [Fact]
    public async Task AddExperience_ShouldReturn404WhenPlayerDoesNotExist()
    {
        // Arrange
        int playerId = 999999;

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/players/{playerId}/experience",
            new { Amount = 100 });

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreatePlayer_ShouldReturn400WhenNameIsEmpty()
    {
        // Arrange
        var request = new { Name = "" };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/players",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreatePlayer_ShouldReturn400WhenNameIsMissing()
    {
        // Arrange
        var request = new { };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/players",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
public record PlayerResponse(
    int Id,
    string Name,
    int Experience,
    int Level);