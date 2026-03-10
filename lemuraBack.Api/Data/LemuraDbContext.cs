using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Models;

namespace LemuraBack.Api.Data;

public class LemuraDbContext : DbContext
{
    public LemuraDbContext(DbContextOptions<LemuraDbContext> options) : base(options) { }

    public DbSet<Room> Rooms => Set<Room>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Room)
            .WithMany()
            .HasForeignKey(b => b.RoomId);

        modelBuilder.Entity<Review>()
            .HasOne(r => r.Booking)
            .WithMany()
            .HasForeignKey(r => r.BookingId);
    }
}