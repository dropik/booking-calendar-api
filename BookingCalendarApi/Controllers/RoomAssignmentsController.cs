using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/room-assignments")]
    [ApiController]
    [Authorize]
    public class RoomAssignmentsController : ControllerBase
    {
        private readonly IRoomAssignmentsService _service;

        public RoomAssignmentsController(IRoomAssignmentsService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IDictionary<string, long?> assignmentRequests)
        {
            await _service.AssignRooms(assignmentRequests);
            return Ok();
        }
    }
}
