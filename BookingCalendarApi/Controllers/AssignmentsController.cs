using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("/api/v1/assignments")]
    [ApiController]
    [Authorize]
    public class AssignmentsController : ControllerBase
    {
        private readonly IAssignmentsService _service;

        public AssignmentsController(IAssignmentsService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> SetAssignments([FromBody] AssignmentsRequest request)
        {
            await _service.Set(request);
            return Ok();
        }

        [HttpPost("colors")]
        public async Task<IActionResult> SetColors([FromBody] IDictionary<string, string> request)
        {
            await _service.SetColors(request);
            return Ok();
        }
    }
}
