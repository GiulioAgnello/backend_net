using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;

namespace LemuraBack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly LemuraDbContext _db;

    public BookingsController(LemuraDbContext db)
    {
        _db = db;
    }

    // GET /api/Bookings/ — tutte le prenotazioni (owner)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var bookings = await _db.Bookings
            .Include(b => b.Room)
            .OrderByDescending(b => b.CreatedAt)
            .Select(b => new {
                b.Id,
                b.GuestName,
                b.GuestEmail,
                b.GuestPhone,
                b.GuestsCount,
                b.CheckIn,
                b.CheckOut,
                b.Status,
                b.Notes,
                b.CreatedAt,
                RoomName = b.Room != null ? b.Room.Name : ""
            })
            .ToListAsync();
        return Ok(bookings);
    }

    // GET /api/Bookings/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _db.Bookings
            .Include(b => b.Room)
            .Where(b => b.Id == id)
            .Select(b => new {
                b.Id,
                b.GuestName,
                b.GuestEmail,
                b.GuestPhone,
                b.GuestsCount,
                b.CheckIn,
                b.CheckOut,
                b.Status,
                b.Notes,
                b.CreatedAt,
                RoomName = b.Room != null ? b.Room.Name : ""
            })
            .FirstOrDefaultAsync();
        if (booking == null) return NotFound();
        return Ok(booking);
    }

    // GET /api/Bookings/room/{roomId}/booked-dates
    [HttpGet("room/{roomId}/booked-dates")]
    public async Task<IActionResult> GetBookedDates(int roomId)
    {
        var dates = await _db.Bookings
            .Where(b => b.RoomId == roomId &&
                        b.Status != "cancelled" &&
                        b.CheckOut >= DateTime.UtcNow)
            .Select(b => new { from = b.CheckIn, to = b.CheckOut })
            .ToListAsync();
        return Ok(dates);
    }

    // POST /api/Bookings/
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest req)
    {
        var room = await _db.Rooms.FindAsync(req.RoomId);
        if (room == null) return BadRequest("Stanza non trovata");

        var booking = new Booking
        {
            RoomId = req.RoomId,
            GuestName = req.GuestName,
            GuestEmail = req.GuestEmail,
            GuestPhone = req.GuestPhone ?? "",
            GuestsCount = req.GuestsCount,
            CheckIn = req.CheckIn,
            CheckOut = req.CheckOut,
            Notes = req.Notes ?? "",
            Status = "pending"
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        // Crea automaticamente un account guest
        if (!await _db.Users.AnyAsync(u => u.Email == req.GuestEmail))
        {
            var tempPassword = BCrypt.Net.BCrypt.HashPassword("guest" + booking.Id);
            _db.Users.Add(new User
            {
                Email = req.GuestEmail,
                PasswordHash = tempPassword,
                Role = "guest",
                Name = req.GuestName
            });
            await _db.SaveChangesAsync();
        }

        return CreatedAtAction(nameof(GetById), new { id = booking.Id }, booking);
    }

    // PATCH /api/Bookings/{id}/status
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req)
    {
        var booking = await _db.Bookings.FindAsync(id);
        if (booking == null) return NotFound();
        booking.Status = req.Status;
        await _db.SaveChangesAsync();
        return Ok(new { message = "Stato aggiornato", status = booking.Status });
    }
}

public class CreateBookingRequest
{
    public int RoomId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string? GuestPhone { get; set; }
    public int GuestsCount { get; set; } = 1;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string? Notes { get; set; }
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}