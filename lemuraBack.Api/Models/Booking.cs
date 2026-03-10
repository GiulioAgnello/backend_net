namespace LemuraBack.Api.Models;

public class Booking
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    public string GuestName { get; set; } = string.Empty;
    public string GuestEmail { get; set; } = string.Empty;
    public string GuestPhone { get; set; } = string.Empty;
    public int GuestsCount { get; set; } = 1;
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public string Status { get; set; } = "pending"; // pending | confirmed | cancelled | checkedin | checkedout
    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}