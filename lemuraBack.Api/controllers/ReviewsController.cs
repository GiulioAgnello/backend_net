using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;

namespace LemuraBack.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly LemuraDbContext _db;

    public ReviewsController(LemuraDbContext db)
    {
        _db = db;
    }

    // GET /api/Reviews/public — recensioni pubbliche (homepage)
    [HttpGet("public")]
    public async Task<IActionResult> GetPublic()
    {
        var reviews = await _db.Reviews
            .Where(r => r.IsPublic)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new {
                r.Id,
                r.GuestName,
                r.Rating,
                r.Comment,
                r.CreatedAt
            })
            .ToListAsync();
        return Ok(reviews);
    }

    // GET /api/Reviews/ — tutte (owner)
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reviews = await _db.Reviews
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        return Ok(reviews);
    }

    // POST /api/Reviews/ — crea recensione (guest)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest req)
    {
        var review = new Review
        {
            BookingId = req.BookingId,
            GuestName = req.GuestName,
            Rating = Math.Clamp(req.Rating, 1, 5),
            Comment = req.Comment ?? "",
            IsPublic = true
        };
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAll), new { id = review.Id }, review);
    }

    // PATCH /api/Reviews/{id}/visibility
    [HttpPatch("{id}/visibility")]
    public async Task<IActionResult> ToggleVisibility(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return NotFound();
        review.IsPublic = !review.IsPublic;
        await _db.SaveChangesAsync();
        return Ok(new { isPublic = review.IsPublic });
    }
}

public class CreateReviewRequest
{
    public int BookingId { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
}