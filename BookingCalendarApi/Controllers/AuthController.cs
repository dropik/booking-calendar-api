using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("token")]
        public async Task<ActionResult<TokenResponse>> Auth([FromBody] TokenRequest request)
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
                    return result;
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenResponse>> Refresh([FromBody] RefreshTokenRequest request)
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
                    return result;
                }
            }
            catch (Exception)
            {
                return Unauthorized();
            }
        }
    }
}
