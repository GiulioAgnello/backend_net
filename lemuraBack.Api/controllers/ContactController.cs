using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;

namespace LemuraBack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly LemuraDbContext _db;

    public ContactController(LemuraDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromBody] Contact req)
    {
        _db.Contacts.Add(req);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Messaggio ricevuto" });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contacts = await _db.Contacts
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
        return Ok(contacts);
    }
}