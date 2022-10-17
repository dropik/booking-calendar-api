using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IBookingComposer _bookingComposer;
        private readonly Func<Services.ISession> _sessionProvider;

        public BookingsController(
            IBookingsProvider bookingsProvider,
            IBookingComposer bookingColorizer,
            Func<Services.ISession> sessionProvider
        )
        {
            _bookingsProvider = bookingsProvider;
            _bookingComposer = bookingColorizer;
            _sessionProvider = sessionProvider;
        }

        [HttpGet]
        public async Task<ActionResult<BookingsControllerResponse>> GetAsync(string from, string to, string? sessionId)
        {
            try
            {
                var session = _sessionProvider();

                await Task.WhenAll(
                    session.OpenAsync(sessionId),
                    _bookingsProvider.FetchBookingsAsync(from, to)
                );

                var bookings = _bookingsProvider.Bookings
                    .SelectInRangeBookings(from, to)
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
