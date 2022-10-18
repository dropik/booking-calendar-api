using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/bookings-by-session")]
    [ApiController]
    public class BookingsBySessionController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IBookingComposer _bookingComposer;
        private readonly Func<Services.ISession> _sessionProvider;

        public BookingsBySessionController(
            IBookingsProvider bookingsProvider,
            IBookingComposer bookingComposer,
            Func<Services.ISession> sessionProvider
        )
        {
            _bookingsProvider = bookingsProvider;
            _bookingComposer = bookingComposer;
            _sessionProvider = sessionProvider;
        }

        [HttpGet]
        public async Task<ActionResult<BookingsControllerResponse>> GetBySessionAsync(string from, string to, string? sessionId)
        {
            try
            {
                var session = _sessionProvider();

                await Task.WhenAll(
                    session.OpenAsync(sessionId),
                    _bookingsProvider.FetchBookingsAsync(from, to)
                );

                var bookings = _bookingsProvider.Bookings
                    .SelectInRangeBookings(from, to, true)
                    .ExcludeBySession(session)
                    .UseComposer(_bookingComposer)
                    .ToList();

                return new BookingsControllerResponse(session.Id.ToString())
                {
                    Bookings = bookings
                };
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
