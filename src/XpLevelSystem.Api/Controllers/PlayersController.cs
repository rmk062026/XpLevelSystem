using Microsoft.AspNetCore.Mvc;
using XpLevelSystem.Core;

namespace XpLevelSystem.Api.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersController : ControllerBase
{
    private readonly PlayerService _service;

    public PlayersController(PlayerService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult CreatePlayer([FromBody] CreatePlayerRequest request)
    {
        try
        {
            int id = _service.CreatePlayer(request.Name);

            var player = _service.GetPlayer(id)!;

            return Created($"/api/players/{id}", new
            {
                Id = id,
                player.Name,
                player.Experience,
                player.Level
            });
        }
        catch (ArgumentException)
        {
            return BadRequest("Spillernavn kan ikke være tomt.");
        }
    }

    [HttpGet("{id:int}")]
    public IActionResult GetPlayer(int id)
    {
        var player = _service.GetPlayer(id);

        if (player == null)
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
    public IActionResult AddExperience(
        int id,
        [FromBody] AddExperienceRequest request)
    {
        try
        {
            var player = _service.AddExperience(id, request.Amount);

            if (player == null)
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
        catch (ArgumentOutOfRangeException)
        {
            return BadRequest("XP må være større enn 0.");
        }
    }
}

public record CreatePlayerRequest(string Name);
public record AddExperienceRequest(int Amount);