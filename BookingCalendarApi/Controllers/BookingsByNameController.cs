using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/bookings-by-name")]
    [ApiController]
    public class BookingsByNameController : ControllerBase
    {
        private readonly IBookingsProvider _bookingsProvider;
        private readonly IBookingShortComposer _bookingShortComposer;

        public BookingsByNameController(IBookingsProvider bookingsProvider, IBookingShortComposer bookingShortComposer)
        {
            _bookingsProvider = bookingsProvider;
            _bookingShortComposer = bookingShortComposer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingShort>>> GetByNameAsync(string from, string to, string? name)
        {
            try
            {
                await _bookingsProvider.FetchBookingsAsync(from, to);

                var definedName = name ?? "";

                var bookings = _bookingsProvider.Bookings
                    .SelectInRange(from, to)
                    .SelectByName(definedName)
                    .UseComposer(_bookingShortComposer)
                    .ToList();

                return bookings;

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}