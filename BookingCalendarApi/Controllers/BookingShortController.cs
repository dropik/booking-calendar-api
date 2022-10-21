using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/booking-short")]
    [ApiController]
    public class BookingShortController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IBookingShortComposer _bookingShortComposer;

        public BookingShortController(IBookingsProvider bookingsProvider, IBookingShortComposer bookingShortComposer)
        {
            _bookingsProvider = bookingsProvider;
            _bookingShortComposer = bookingShortComposer;
        }

        [HttpGet]
        public async Task<ActionResult<ShortBooking>> GetAsync(string id, string from)
        {
            try
            {
                var to = DateTime.ParseExact(from, "yyyy-MM-dd", null).AddDays(1).ToString("yyyy-MM-dd");
                await _bookingsProvider.FetchBookingsAsync(from, to, exactPeriod: true);

                var booking = _bookingsProvider.Bookings
                    .SelectById(id)
                    .UseComposer(_bookingShortComposer);

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