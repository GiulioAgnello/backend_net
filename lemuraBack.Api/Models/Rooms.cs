namespace LemuraBack.Api.Models
{
    public class Rooms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
    }
}