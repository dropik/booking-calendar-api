using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingsController(IBookingService service)
        {
            _service = service;
        }

        [HttpGet("booking")]
        public async Task<ActionResult<Booking<List<Client>>>> Get(string id, string from)
        {
            return Ok(await _service.Get(id, from));
        }

        [HttpGet("bookings-by-name")]
        public async Task<ActionResult<List<ShortBooking>>> GetByName(string from, string to, string? name)
        {
            return Ok(await _service.GetByName(from, to, name));
        }

        [HttpGet("bookings-by-session")]
        public async Task<ActionResult<BookingsBySessionResponse>> GetBySession(string from, string to, string? sessionId)
        {
            return Ok(await _service.GetBySession(from, to, sessionId));
        }

        [HttpPost("ack-bookings")]
        public async Task<IActionResult> Post(AckBookingsRequest request)
        {
            await _service.Ack(request);
            return Ok();
        }
    }
}