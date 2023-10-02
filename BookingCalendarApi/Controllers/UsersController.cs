using BookingCalendarApi.Models.Requests.Users;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = Repository.User.ADMIN_ROLE)]
        public async Task<ActionResult<Models.Responses.CreatedResult>> Create(CreateUserRequest request)
        {
            var result = await _service.Create(request);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpGet("current")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            return await _service.GetCurrentUser();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = Repository.User.ADMIN_ROLE)]
        public async Task<ActionResult<UserResponse>> Get(long id)
        {
            return Ok(await _service.Get(id));
        }

        [HttpPatch("current/visible-name")]
        public async Task<IActionResult> UpdateVisibleName([FromBody] UpdateVisibleNameRequest request)
        {
            await _service.UpdateVisibleName(request);
            return Ok();
        }

        [HttpPatch("current/password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            await _service.UpdatePassword(request);
            return Ok();
        }
    }
}
