using BookingCalendarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/ack-bookings")]
    [ApiController]
    public class AckBookingsController : ControllerBase
    {
        private readonly BookingCalendarContext _context;
        private readonly Func<Services.ISession> _sessionProvider;

        public AckBookingsController(BookingCalendarContext context, Func<Services.ISession> sessionProvider)
        {
            _context = context;
            _sessionProvider = sessionProvider;
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
                var session = _sessionProvider();
                await session.OpenAsync(sessionId);
                session.WriteNewData(bookings);
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
