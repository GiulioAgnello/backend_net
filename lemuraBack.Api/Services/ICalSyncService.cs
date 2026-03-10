using LemuraBack.Api.Data;
using LemuraBack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LemuraBack.Api.Services;

public class ICalSyncService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ICalSyncService> _logger;
    private readonly HttpClient _http;

    public ICalSyncService(IServiceProvider services, ILogger<ICalSyncService> logger)
    {
        _services = services;
        _logger = logger;
        _http = new HttpClient();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SyncAll();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    public async Task SyncAll()
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LemuraDbContext>();

        var rooms = await db.Rooms.ToListAsync();

        foreach (var room in rooms)
        {
            if (!string.IsNullOrEmpty(room.BookingIcalUrl))
                await SyncIcal(db, room, room.BookingIcalUrl, "booking.com");

            if (!string.IsNullOrEmpty(room.AirbnbIcalUrl))
                await SyncIcal(db, room, room.AirbnbIcalUrl, "airbnb");
        }

        await db.SaveChangesAsync();
    }

    private async Task SyncIcal(LemuraDbContext db, Room room, string url, string source)
    {
        try
        {
            var ical = await _http.GetStringAsync(url);
            var events = ParseIcal(ical);

            foreach (var (start, end, summary) in events)
            {
                // Evita duplicati
                var exists = await db.Bookings.AnyAsync(b =>
                    b.RoomId == room.Id &&
                    b.CheckIn == start &&
                    b.CheckOut == end &&
                    b.Notes == $"[{source}]");

                if (!exists)
                {
                    db.Bookings.Add(new Booking
                    {
                        RoomId = room.Id,
                        GuestName = summary ?? source,
                        GuestEmail = $"sync@{source}.com",
                        CheckIn = start,
                        CheckOut = end,
                        Status = "confirmed",
                        Notes = $"[{source}]",
                        GuestsCount = 1
                    });
                }
            }

            _logger.LogInformation("Sync {source} per stanza {room}: {count} eventi", source, room.Name, events.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError("Errore sync {source} stanza {room}: {msg}", source, room.Name, ex.Message);
        }
    }

    private List<(DateTime start, DateTime end, string? summary)> ParseIcal(string ical)
    {
        var events = new List<(DateTime, DateTime, string?)>();
        var lines = ical.Replace("\r\n", "\n").Replace("\r", "\n").Split('\n');

        bool inEvent = false;
        DateTime? start = null, end = null;
        string? summary = null;

        foreach (var line in lines)
        {
            if (line == "BEGIN:VEVENT") { inEvent = true; start = end = null; summary = null; }
            else if (line == "END:VEVENT")
            {
                if (inEvent && start.HasValue && end.HasValue)
                    events.Add((start.Value, end.Value, summary));
                inEvent = false;
            }
            else if (inEvent)
            {
                if (line.StartsWith("DTSTART"))
                    start = ParseIcalDate(line.Split(':')[1]);
                else if (line.StartsWith("DTEND"))
                    end = ParseIcalDate(line.Split(':')[1]);
                else if (line.StartsWith("SUMMARY:"))
                    summary = line.Substring(8);
            }
        }

        return events;
    }

    private DateTime? ParseIcalDate(string value)
    {
        value = value.Trim();
        if (DateTime.TryParseExact(value, "yyyyMMdd", null,
            System.Globalization.DateTimeStyles.None, out var d))
            return d;
        if (DateTime.TryParseExact(value, "yyyyMMddTHHmmssZ", null,
            System.Globalization.DateTimeStyles.AssumeUniversal, out var dt))
            return dt.ToUniversalTime();
        return null;
    }
}