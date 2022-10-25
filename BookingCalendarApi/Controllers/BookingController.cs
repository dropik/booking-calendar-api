using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IBookingComposer _bookingComposer;

        public BookingController(IBookingsProvider bookingsProvider, IBookingComposer bookingComposer)
        {
            _bookingsProvider = bookingsProvider;
            _bookingComposer = bookingComposer;
        }

        [HttpGet]
        public async Task<ActionResult<Booking>> GetAsync(string id, string from)
        {
            try
            {
                var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
                var arrivalFrom = fromDate.AddDays(-15).ToString("yyyy-MM-dd");
                var arrivalTo = fromDate.AddDays(15).ToString("yyyy-MM-dd");
                await _bookingsProvider.FetchBookingsAsync(arrivalFrom, arrivalTo, exactPeriod: true);

                var booking = _bookingsProvider.Bookings
                    .SelectById(id)
                    .UseComposer(_bookingComposer);

                if (!booking.Any())
                {
                    return NotFound();
                }

                return booking.First();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}