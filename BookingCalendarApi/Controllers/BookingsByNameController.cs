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
        private readonly IBookingShortComposer _bookingShortComposer;

        public BookingsByNameController(IIperbooking iperbooking, IBookingShortComposer bookingShortComposer)
        {
            _iperbooking = iperbooking;
            _bookingShortComposer = bookingShortComposer;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShortBooking>>> GetByNameAsync(string from, string to, string? name)
        {
            try
            {
                var definedName = name ?? "";
                return (await _iperbooking.GetBookingsAsync(from, to))
                    .SelectInRange(from, to)
                    .SelectByName(definedName)
                    .UseComposer(_bookingShortComposer)
                    .ToList();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}