using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/istat")]
    [ApiController]
    public class IstatController : ControllerBase
    {
        private readonly IIstatService _serviseSession;

        public IstatController(IIstatService serviceSession)
        {
            _serviseSession = serviceSession;
        }

        [HttpGet]
        public async Task<ActionResult<IstatLastDateResponse>> GetLastDate()
        {
            return Ok(await _serviseSession.GetLastDate());
        }

        [HttpPost]
        public async Task<IActionResult> SendNewData(IstatSendDataRequest request)
        {
            await _serviseSession.SendNewData(request);
            return Ok();
        }
    }
}
