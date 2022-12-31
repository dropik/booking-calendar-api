using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/bookings-by-session")]
    [ApiController]
    public class BookingsBySessionController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;
        private readonly IBookingsCachingSession _session;
        private readonly BookingCalendarContext _context;

        private List<Booking> Bookings { get; set; } = new();

        public BookingsBySessionController(
            IIperbooking iperbooking,
            IBookingsCachingSession session,
            BookingCalendarContext context)
        {
            _iperbooking = iperbooking;
            _session = session;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BookingsBySession>> GetBySessionAsync(string from, string to, string? sessionId)
        {
            try
            {
                await Task.WhenAll(
                    _session.OpenAsync(sessionId),
                    FetchBookings(from, to)
                );

                var bookings = Bookings
                    .SelectInRange(from, to, true)
                    .ExcludeBySession(_session);

                var bookingsWithColors = from booking in bookings
                                         join color in _context.ColorAssignments on booking.BookingNumber.ToString() equals color.BookingId into gj
                                         from color in gj.DefaultIfEmpty()
                                         select new { Booking = booking, Color = color };

                var bookingsWithGuestsCount = bookingsWithColors.Select(join => new Booking<uint>(
                        id: join.Booking.BookingNumber.ToString(),
                        name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                        lastModified: join.Booking.LastModified,
                        from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"))
                    {
                        Status = join.Booking.Status,
                        Color = join.Color?.Color,
                        Tiles = (from room in @join.Booking.Rooms
                                 join assignment in _context.RoomAssignments on $"{room.StayId}-{room.Arrival}-{room.Departure}" equals assignment.Id into gj
                                 from assignment in gj.DefaultIfEmpty()
                                 select new { Room = room, Assignment = assignment })
                                     .ToList()
                                     .Select(join => new Tile<uint>(
                                        id: $"{join.Room.StayId}-{join.Room.Arrival}-{join.Room.Departure}",
                                        from: DateTime.ParseExact(join.Room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                        nights: Convert.ToUInt32((DateTime.ParseExact(join.Room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(join.Room.Arrival, "yyyyMMdd", null)).Days),
                                        roomType: join.Room.RoomName,
                                        rateId: join.Room.RateId,
                                        persons: Convert.ToUInt32(join.Room.Guests.Count()))
                                     {
                                         RoomId = join.Assignment?.RoomId ?? null
                                     })
                                     .ToList()
                    })
                    .ToList();

                return new BookingsBySession(_session.Id.ToString())
                {
                    Bookings = bookingsWithGuestsCount
                };
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task FetchBookings(string from, string to)
        {
            Bookings = await _iperbooking.GetBookingsAsync(from, to);
        }
    }
}
