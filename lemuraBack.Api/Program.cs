using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LemuraBack.Api.Data;
using LemuraBack.Api.Models;
using LemuraBack.Api.Services;

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
builder.Services.AddHostedService<ICalSyncService>();
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
            // STERNATIA — casa intera
        new Room {
            Name = "Sternatia — Casa Intera",
            Description = "Casa intera nel palazzo del '600. Soggiorno, cucina attrezzata, bagno e camere. Perfetta per famiglie o gruppi.",
            PricePerNight = 180m,
            IsAvailable = true,
            MaxGuests = 6
        },
        // CORIGLIANO — Suite 1
        new Room {
            Name = "Corigliano — Suite Castello",
            Description = "Suite con vista sul castello angioino. Camera matrimoniale con bagno privato e terrazzo.",
            PricePerNight = 120m,
            IsAvailable = true,
            MaxGuests = 4
        },
        // CORIGLIANO — Suite 2
        new Room {
            Name = "Corigliano — Suite Giardino",
            Description = "Suite affacciata sul giardino interno. Camera matrimoniale o doppia con bagno privato.",
            PricePerNight = 110m,
            IsAvailable = true,
            MaxGuests = 4
        }
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