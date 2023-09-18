using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/structures")]
    [ApiController]
    [Authorize]
    public class StructuresController : ControllerBase
    {
        private readonly IStructureService _service;

        public StructuresController(IStructureService service)
        {
            _service = service;
        }

        [HttpGet("current/api-keys")]
        public async Task<ActionResult<APIKeysResponse>> GetApiKeys()
        {
            return Ok(await _service.GetApiKeys());
        }

        [HttpPatch("current/api-keys")]
        public async Task<IActionResult> UpdateApiKeys([FromBody] UpdateAPIKeysRequest request)
        {
            await _service.UpdateApiKeys(request);
            return Ok();
        }
    }
}
