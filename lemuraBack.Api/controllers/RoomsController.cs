using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;
using LemuraBack.Api.Services;
namespace LemuraBack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly LemuraDbContext _db;

    public RoomsController(LemuraDbContext db)
    {
        _db = db;
    }

    // GET /api/Rooms/
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var rooms = await _db.Rooms
            .Where(r => r.IsAvailable)
            .Select(r => new {
                r.Id,
                r.Name,
                r.Description,
                r.PricePerNight,
                r.MaxGuests,
                r.IsAvailable
            })
            .ToListAsync();
        return Ok(rooms);
    }

    // GET /api/Rooms/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room == null) return NotFound();
        return Ok(room);
    }

    // PATCH /api/Rooms/{id}/ical — aggiorna link iCal
    [HttpPatch("{id}/ical")]
    public async Task<IActionResult> UpdateIcal(int id, [FromBody] UpdateIcalRequest req)
    {
        var room = await _db.Rooms.FindAsync(id);
        if (room == null) return NotFound();

        if (req.BookingIcalUrl != null) room.BookingIcalUrl = req.BookingIcalUrl;
        if (req.AirbnbIcalUrl != null) room.AirbnbIcalUrl = req.AirbnbIcalUrl;

        await _db.SaveChangesAsync();

        // Sync immediata dopo aver salvato i link
        return Ok(new { message = "Link iCal aggiornati" });
    }

    // POST /api/Rooms/sync — forza sync manuale
    [HttpPost("sync")]
    public async Task<IActionResult> ForceSync(
        [FromServices] ICalSyncService syncService)
    {
        await syncService.SyncAll();
        return Ok(new { message = "Sync completata" });
    }
}

public class UpdateIcalRequest
{
    public string? BookingIcalUrl { get; set; }
    public string? AirbnbIcalUrl { get; set; }
}