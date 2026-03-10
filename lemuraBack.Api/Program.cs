using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// SQLite
builder.Services.AddDbContext<LemuraDbContext>(options =>
    options.UseSqlite("Data Source=lemura.db"));

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "LeMuraAngeli2024SecretKeyVeryLong!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
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
            new Room { Name = "Sternatia — Camera Matrimoniale", Description = "Camera matrimoniale con vista sul borgo medievale", PricePerNight = 120m, IsAvailable = true },
            new Room { Name = "Sternatia — Camera Doppia", Description = "Camera doppia nel palazzo del '600", PricePerNight = 100m, IsAvailable = true },
            new Room { Name = "Corigliano — Camera Matrimoniale", Description = "Camera con vista sul castello angioino", PricePerNight = 110m, IsAvailable = true }
        );
        db.SaveChanges();
    }
}

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();