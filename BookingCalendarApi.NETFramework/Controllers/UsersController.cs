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

        [HttpGet]
        [Route("current")]
        public async Task<IHttpActionResult> GetCurrentUser()
        {
            return Ok(await _service.GetCurrentUser());
        }
    }
}
