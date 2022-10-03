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
                string[] dateFormats = new[] { "yyyy-MM-dd" };
                var fromDate = DateTime.ParseExact(from, dateFormats, null);
                var toDate = DateTime.ParseExact(to, dateFormats, null);

                var arrivalFromDate = fromDate.AddDays(-30);

                var arrivalFrom = arrivalFromDate.ToString("yyyyMMdd");
                var arrivalTo = toDate.ToString("yyyyMMdd");

                var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);

                var guid = sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();
                var tiles = GetTilesFromBookings(bookings, fromDate)
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

        private IEnumerable<Tile> GetTilesFromBookings(ICollection<Models.Iperbooking.Bookings.Booking> bookings, DateTime fromDate)
        {
            string[] dateFormat = new[] { "yyyyMMdd" };
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
                        var arrivalDate = DateTime.ParseExact(room.Arrival, dateFormat, null);
                        newTile.From = arrivalDate.ToString("yyyy-MM-dd");

                        var departureDate = DateTime.ParseExact(room.Departure, dateFormat, null);

                        if ((departureDate - fromDate).Days < 0)
                        {
                            continue;
                        }

                        newTile.Nights = Convert.ToUInt32((departureDate - arrivalDate).Days);

                        newTile.Color = $"booking{(random.Next() % 8) + 1}";
                    } catch (Exception)
                    {
                        continue;
                    }
                    
                    yield return newTile;
                }
            }
        }
    }
}