using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;

namespace LemuraBack.Api.Controllers;

[ApiController]
[Route("api/rooms")]
public class RoomsController : ControllerBase
{
    private readonly AppDbContext _context;

    public RoomsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/rooms
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Room>>> GetAll()
    {
        return await _context.Rooms.ToListAsync();
    }

    // GET: api/rooms/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Room>> GetById(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        return room;
    }

    // DELETE: api/rooms/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // PUT: api/rooms/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Room updatedRoom)
    {
        if (id != updatedRoom.Id)
        {
            return BadRequest("ID mismatch");
        }

        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            return NotFound();
        }

        room.Name = updatedRoom.Name;
        room.Description = updatedRoom.Description;
        room.PricePerNight = updatedRoom.PricePerNight;
        room.IsAvailable = updatedRoom.IsAvailable;

        await _context.SaveChangesAsync();

        return NoContent();
    }
}