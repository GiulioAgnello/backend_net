using Microsoft.EntityFrameworkCore;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<LemuraDbContext>(options =>
    options.UseSqlite("Data Source=lemura.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LemuraDbContext>();
    db.Database.EnsureCreated();
    
    if (!db.Rooms.Any())
    {
        db.Rooms.AddRange(
            new Room { Name = "Deluxe Room", Description = "Stanza lussuosa con vista", PricePerNight = 150m, IsAvailable = true },
            new Room { Name = "Standard Room", Description = "Stanza confortevole", PricePerNight = 80m, IsAvailable = true },
            new Room { Name = "Budget Room", Description = "Stanza economica", PricePerNight = 50m, IsAvailable = false }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
