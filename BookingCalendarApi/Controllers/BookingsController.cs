using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/bookings")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingsService _service;

        public BookingsController(IBookingsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookingResponse<uint>>>> GetBySession([FromQuery] string from, [FromQuery] string to)
        {
            return Ok(await _service.GetByPeriod(from, to));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingResponse<List<ClientResponse>>>> Get(string id, [FromQuery] string from)
        {
            return Ok(await _service.Get(id, from));
        }

        [HttpGet("by-name")]
        public async Task<ActionResult<List<ShortBookingResponse>>> GetByName([FromQuery] string from, [FromQuery] string to, [FromQuery] string? name)
        {
            return Ok(await _service.GetByName(from, to, name));
        }
    }
}