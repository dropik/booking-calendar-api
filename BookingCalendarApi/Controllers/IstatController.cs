using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IstatController : ControllerBase
    {
        private readonly IC59ServiceSession _serviseSession;

        public IstatController(IC59ServiceSession serviceSession)
        {
            _serviseSession = serviceSession;
        }

        [HttpGet]
        public async Task<ActionResult<IstatLastDateResponse>> GetLastDateAsync()
        {
            return Ok(await _serviseSession.GetLastDateAsync());
        }

        [HttpPost]
        public async Task<IActionResult> SendNewDataAsync(IstatSendDataRequest request)
        {
            await _serviseSession.SendNewDataAsync(DateTime.ParseExact(request.Date, "yyyy-MM-dd", null));
            return Ok();
        }
    }
}
