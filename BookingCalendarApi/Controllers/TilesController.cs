using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;
        private readonly BookingCalendarContext _context;

        public TilesController(IIperbooking iperbooking, BookingCalendarContext context)
        {
            _iperbooking = iperbooking;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<TileResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
                var toDate = DateTime.ParseExact(to, "yyyy-MM-dd", null);

                var arrivalFromDate = fromDate.AddDays(-30);

                var arrivalFrom = arrivalFromDate.ToString("yyyyMMdd");
                var arrivalTo = toDate.ToString("yyyyMMdd");

                var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);

                var random = new Random();

                var guid = sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();
                var tiles = GetTilesFromBookings(bookings)
                    .Where(tile => (DateTime.ParseExact(tile.From, "yyyy-MM-dd", null) - fromDate).Days >= -tile.Nights)
                    .Where(tile => !_context.Sessions.Contains(new Session { Id = guid, TileId = tile.Id }))
                    .GroupJoin(
                        _context.TileAssignments,
                        tile => tile.Id,
                        assignment => assignment.Id,
                        (tile, assignments) => new { tile, assignments }
                    )
                    .SelectMany(
                        x => x.assignments,
                        (tileJoin, assignment) => new Tile {
                            Id = tileJoin.tile.Id,
                            BookingId = tileJoin.tile.BookingId,
                            Name = tileJoin.tile.Name,
                            From = tileJoin.tile.From,
                            Nights = tileJoin.tile.Nights,
                            RoomType = tileJoin.tile.RoomType,
                            Entity = tileJoin.tile.Entity,
                            Persons = tileJoin.tile.Persons,
                            Color = assignment?.Color ?? $"booking{(random.Next() % 8) + 1}",
                            RoomId = assignment?.RoomId ?? null
                        }
                    )
                    .ToList();

                foreach (var tile in tiles)
                {
                    _context.Sessions.Add(new Session { Id = guid, TileId = tile.Id });
                    if (!_context.TileAssignments.Any(a => a.Id.Equals(tile.Id)))
                    {
                        _context.TileAssignments.Add(new TileAssignment(tile.Id) { Color = tile.Color, RoomId = tile.RoomId });
                    }
                }

                await _context.SaveChangesAsync();

                return new TileResponse
                {
                    Tiles = tiles,
                    SessionId = guid.ToString()
                };
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private IEnumerable<Tile> GetTilesFromBookings(ICollection<Models.Iperbooking.Bookings.Booking> bookings)
        {
            var random = new Random();

            foreach (var booking in bookings)
            {
                foreach (var room in booking.Rooms)
                {
                    Tile newTile = new()
                    {
                        Id = room.StayId.ToString(),
                        BookingId = booking.BookingNumber.ToString(),
                        Name = $"{booking.FirstName} {booking.LastName}",
                        RoomType = room.RoomName,
                        Entity = room.RoomName,
                        Persons = Convert.ToUInt32(room.Guests.Count)
                    };

                    try
                    {
                        var arrivalDate = DateTime.ParseExact(room.Arrival, "yyyyMMdd", null);
                        newTile.From = arrivalDate.ToString("yyyy-MM-dd");

                        var departureDate = DateTime.ParseExact(room.Departure, "yyyyMMdd", null);
                        newTile.Nights = Convert.ToUInt32((departureDate - arrivalDate).Days);
                    } catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                    
                    yield return newTile;
                }
            }
        }
    }
}