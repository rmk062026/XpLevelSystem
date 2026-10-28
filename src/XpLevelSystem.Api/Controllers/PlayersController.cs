
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using XpLevelSystem.Core;

namespace XpLevelSystem.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/players")]
public class PlayersController : ControllerBase
{
    private readonly PlayerService _service;

    public PlayersController(PlayerService service)
    {
        _service = service;
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new UnauthorizedAccessException("Ugyldig bruker-ID.");
        }

        return parsedUserId;
    }

    [HttpPost]
    public IActionResult CreatePlayer([FromBody] CreatePlayerRequest request)
    {
        try
        {
            var userId = GetUserId();

            int id = _service.CreatePlayer(userId, request.Name);

            var player = _service.GetPlayer(userId, id)!;

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
        var player = _service.GetPlayer(GetUserId(), id);

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
            var player = _service.AddExperience(
                GetUserId(), id, request.Amount);

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

    [HttpDelete("{id:int}")]
    public IActionResult DeletePlayer(int id)
    {
        var deleted = _service.DeletePlayer(GetUserId(), id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

public record CreatePlayerRequest(string Name);
public record AddExperienceRequest(int Amount);
