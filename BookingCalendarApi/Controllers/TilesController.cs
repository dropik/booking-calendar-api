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

                var guid = sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();
                var tiles = GetTilesFromBookings(bookings)
                    .Where(tile => (DateTime.ParseExact(tile.From, "yyyy-MM-dd", null) - fromDate).Days >= -tile.Nights)
                    .Where(tile => !_context.Sessions.Contains(new Session { Id = guid, TileId = tile.Id }))
                    .ToList();

                foreach (var tile in tiles)
                {
                    _context.Sessions.Add(new Session { Id = guid, TileId = tile.Id });
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

                        var previouslyAssignedQuery = _context.TileAssignments.Where(a => a.Id.Equals(newTile.Id));
                        string color;
                        long? roomId;
                        if (previouslyAssignedQuery.Any())
                        {
                            var previouslyAssigned = previouslyAssignedQuery.First();
                            color = previouslyAssigned.Color;
                            roomId = previouslyAssigned.RoomId;
                        } else
                        {
                            color = $"booking{(random.Next() % 8) + 1}";
                            roomId = null;
                            newTile.Color = color;
                            _context.TileAssignments.Add(new TileAssignment(newTile.Id) { Color = color });
                        }
                        newTile.Color = color;
                        newTile.RoomId = roomId;
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