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
        private readonly IBookingComposer _bookingComposer;
        private readonly IBookingsCachingSession _session;

        private List<Booking> Bookings { get; set; } = new();

        public BookingsBySessionController(
            IIperbooking iperbooking,
            IBookingComposer bookingComposer,
            IBookingsCachingSession session)
        {
            _iperbooking = iperbooking;
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
                    FetchBookings(from, to)
                );

                var bookings = Bookings
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

        private async Task FetchBookings(string from, string to)
        {
            Bookings = await _iperbooking.GetBookingsAsync(from, to);
        }
    }
}
