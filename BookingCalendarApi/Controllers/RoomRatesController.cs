using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/room-rates")]
    [ApiController]
    [Authorize]
    public class RoomRatesController : ControllerBase
    {
        private readonly IRoomRatesService _service;

        public RoomRatesController(IRoomRatesService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<RoomRatesResponse>> GetAsync()
        {
            return Ok(await _service.Get());
        }
    }
}
