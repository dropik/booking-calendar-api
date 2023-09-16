using BookingCalendarApi.Models.Requests;
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
    }
}
