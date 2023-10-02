using BookingCalendarApi.Models.Requests.Users;
using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Services;

using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [RoutePrefix("api/v1/users")]
    [JwtAuthentication]
    public class UsersController : ApiController
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("")]
        [Admin]
        public async Task<IHttpActionResult> Create(CreateUserRequest request)
        {
            var result = await _service.Create(request);
            return CreatedAtRoute("GetUser", new { id = result.Id }, result);
        }

        [HttpGet]
        [Route("{id}", Name = "GetUser")]
        [Admin]
        public async Task<IHttpActionResult> Get(long id)
        {
            return Ok(await _service.Get(id));
        }

        [HttpGet]
        [Route("current")]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            return Ok(await _service.GetCurrentUser());
        }

        [HttpPatch]
        [Route("current/visible-name")]
        public async Task<IHttpActionResult> UpdateVisibleName([FromBody] UpdateVisibleNameRequest request)
        {
            await _service.UpdateVisibleName(request);
            return Ok();
        }

        [HttpPatch]
        [Route("current/password")]
        public async Task<IHttpActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            await _service.UpdatePassword(request);
            return Ok();
        }
    }
}
