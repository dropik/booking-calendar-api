using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/ack-bookings")]
    [ApiController]
    public class AckBookingsController : ControllerBase
    {
        private readonly IAckBookingsService _service;

        public AckBookingsController(IAckBookingsService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post(AckBookingsRequest request)
        {
            await _service.Ack(request);
            return Ok();
        }
    }
}
