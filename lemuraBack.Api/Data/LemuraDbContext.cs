using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Models;

namespace LemuraBack.Api.Data;

public class LemuraDbContext : DbContext
{
    public LemuraDbContext(DbContextOptions<LemuraDbContext> options) : base(options) { }

    public DbSet<Room> Rooms { get; set; }
}
