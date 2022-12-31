using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/bookings-by-name")]
    [ApiController]
    public class BookingsByNameController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;
        private readonly BookingCalendarContext _context;

        public BookingsByNameController(IIperbooking iperbooking, BookingCalendarContext context)
        {
            _iperbooking = iperbooking;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ShortBooking>>> GetByNameAsync(string from, string to, string? name)
        {
            try
            {
                var definedName = name ?? "";
                var bookingsByName = (await _iperbooking.GetBookingsAsync(from, to))
                    .SelectInRange(from, to)
                    .SelectByName(definedName);

                var bookingsWithColors = from booking in bookingsByName
                                         join color in _context.ColorAssignments on booking.BookingNumber.ToString() equals color.BookingId into gj
                                         from color in gj.DefaultIfEmpty()
                                         select new { Booking = booking, Color = color };

                return bookingsWithColors
                    .Select(join => new ShortBooking(
                        id: join.Booking.BookingNumber.ToString(),
                        name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                        lastModified: join.Booking.LastModified,
                        from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                        occupations: Convert.ToUInt32(join.Booking.Rooms.Count)
                    )
                    {
                        Status = join.Booking.Status,
                        Color = join.Color?.Color
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}