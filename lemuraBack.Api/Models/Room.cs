namespace LemuraBack.Api.Models;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal PricePerNight { get; set; }
    public bool IsAvailable { get; set; }
}