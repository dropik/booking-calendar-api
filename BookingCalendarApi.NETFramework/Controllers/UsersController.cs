using BookingCalendarApi.Services;
using System.Threading.Tasks;

#if NET
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/users")]
    [Authorize]
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
