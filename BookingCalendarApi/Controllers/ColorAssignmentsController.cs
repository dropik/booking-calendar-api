using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/color-assignments")]
    [ApiController]
    public class ColorAssignmentsController : ControllerBase
    {
        private readonly IColorAssignmentsService _service;

        public ColorAssignmentsController(IColorAssignmentsService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IDictionary<string, string> colors)
        {
            await _service.AssignColors(colors);
            return Ok();
        }
    }
}