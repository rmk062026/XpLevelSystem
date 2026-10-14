using Microsoft.AspNetCore.Mvc;
using XpLevelSystem.Core;

namespace XpLevelSystem.Api.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersController : ControllerBase
{
    private static readonly Dictionary<int, Player> Players = new();
    private static int _nextId = 1;

    [HttpPost]
    public IActionResult CreatePlayer([FromBody] CreatePlayerRequest request)
    {
        int id = _nextId++;
        var player = new Player(request.Name);

        Players.Add(id, player);

        return Created(
            $"/api/players/{id}",
            new
            {
                Id = id,
                player.Name,
                player.Experience,
                player.Level
            });
    }

    [HttpGet("{id:int}")]
    public IActionResult GetPlayer(int id)
    {
        if (!Players.TryGetValue(id, out var player))
        {
            return NotFound();
        }

        return Ok(new
        {
            Id = id,
            player.Name,
            player.Experience,
            player.Level
        });
    }

    [HttpPost("{id:int}/experience")]
    public IActionResult AddExperience(int id, [FromBody] AddExperienceRequest request)
    {
        if (!Players.TryGetValue(id, out var player))
        {
            return NotFound();
        }

        player.AddExperience(request.Amount);

        return Ok(new
        {
            Id = id,
            player.Name,
            player.Experience,
            player.Level
        });
    }
}

public record CreatePlayerRequest(string Name);
public record AddExperienceRequest(int Amount);