namespace LemuraBack.Api.Models;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
    public int MaxGuests { get; set; } = 2;
    public string? BookingIcalUrl { get; set; }  // link .ics da Booking.com
    public string? AirbnbIcalUrl { get; set; }   // link .ics da Airbnb
}