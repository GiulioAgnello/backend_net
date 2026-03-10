using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;

namespace LemuraBack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly LemuraDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(LemuraDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
            return Unauthorized("Credenziali non valide");

        var token = GenerateToken(user);

        // Se è un guest, trova la sua prenotazione attiva
        int? bookingId = null;
        if (user.Role == "guest")
        {
            var booking = await _db.Bookings
                .Where(b => b.GuestEmail == user.Email &&
                            b.CheckIn <= DateTime.UtcNow &&
                            b.CheckOut >= DateTime.UtcNow)
                .FirstOrDefaultAsync();
            bookingId = booking?.Id;
        }

        return Ok(new
        {
            token,
            role = user.Role,
            name = user.Name,
            email = user.Email,
            bookingId
        });
    }

    [HttpPost("register-owner")]
    public async Task<IActionResult> RegisterOwner([FromBody] LoginRequest req)
    {
        // Endpoint da usare UNA SOLA VOLTA per creare l'account owner
        if (await _db.Users.AnyAsync(u => u.Role == "owner"))
            return BadRequest("Owner già esistente");

        var user = new User
        {
            Email = req.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password),
            Role = "owner",
            Name = req.Name ?? "Proprietario"
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Owner creato con successo" });
    }

    private string GenerateToken(User user)
    {
        var secret = _config["Jwt:Secret"] ?? "LeM uraAngeli2024SecretKeyVeryLong!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Name { get; set; }
}