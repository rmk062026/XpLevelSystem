
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XpLevelSystem.Api.Data;
using XpLevelSystem.Api.DTOs;

namespace XpLevelSystem.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Brukernavn og passord er påkrevd.");
        }

        var username = request.Username.Trim();

        if (username.Length > 100)
        {
            return BadRequest("Brukernavnet er for langt.");
        }

        if (request.Password.Length < 8)
        {
            return BadRequest("Passordet må ha minst 8 tegn.");
        }

        var exists = await _context.Users.AnyAsync(
            u => u.Username.ToLower() == username.ToLower());

        if (exists)
        {
            return Conflict("Brukernavnet er allerede i bruk.");
        }

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.Password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            user.Id,
            user.Username
        });
    }
}
