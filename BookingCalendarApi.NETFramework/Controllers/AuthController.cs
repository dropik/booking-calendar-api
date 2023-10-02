using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Services;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [RoutePrefix("api/v1/auth")]
    public class AuthController : ApiController
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost]
        [Route("token")]
        public async Task<IHttpActionResult> Auth([FromBody] TokenRequest request)
        {
            try
            {
                var result = await _service.GetToken(request);
                if (result == null)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("refresh")]
        public async Task<IHttpActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var result = await _service.GetToken(request);
                if (result == null)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(result);
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
