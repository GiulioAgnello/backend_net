using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Models;

namespace LemuraBack.Api.Data

{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Rooms> Rooms { get; set; }
    }
}