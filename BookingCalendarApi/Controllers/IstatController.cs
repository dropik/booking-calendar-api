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
            try
            {
                var response = await _serviseSession.GetLastDateAsync();
                return new IstatLastDateResponse(response);
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendNewDataAsync(IstatSendDataRequest request)
        {
            try
            {
                await _serviseSession.SendNewDataAsync(DateTime.ParseExact(request.Date, "yyyy-MM-dd", null));
                return Ok();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
