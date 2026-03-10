namespace LemuraBack.Api.Models;

public class Review
{
    public int Id { get; set; }
    public int BookingId { get; set; }
    public Booking? Booking { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}