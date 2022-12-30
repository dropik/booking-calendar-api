using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/ack-bookings")]
    [ApiController]
    public class AckBookingsController : ControllerBase
    {
        private readonly BookingCalendarContext _context;
        private readonly IBookingsCachingSession _session;

        public AckBookingsController(BookingCalendarContext context, IBookingsCachingSession session)
        {
            _context = context;
            _session = session;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(AckBookingsRequest request)
        {
            var bookings = request.Bookings;
            var sessionId = request.SessionId;

            if (bookings == null)
            {
                return Ok();
            }

            try
            {
                await _session.OpenAsync(sessionId);
                _session.WriteNewData(bookings);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
