using BookingCalendarApi.Models;
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
        private readonly IBookingsCachingSession _session;

        public BookingsBySessionController(
            IBookingsProvider bookingsProvider,
            IBookingComposer bookingComposer,
            IBookingsCachingSession session)
        {
            _bookingsProvider = bookingsProvider;
            _bookingComposer = bookingComposer;
            _session = session;
        }

        [HttpGet]
        public async Task<ActionResult<BookingsBySession>> GetBySessionAsync(string from, string to, string? sessionId)
        {
            try
            {
                await Task.WhenAll(
                    _session.OpenAsync(sessionId),
                    _bookingsProvider.FetchBookingsAsync(from, to)
                );

                var bookings = _bookingsProvider.Bookings
                    .SelectInRange(from, to, true)
                    .ExcludeBySession(_session)
                    .UseComposer(_bookingComposer)
                    .ToList();

                return new BookingsBySession(_session.Id.ToString())
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
