using BookingCalendarApi.Models.DTO;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/istat")]
    [ApiController]
    [Authorize]
    public class IstatController : ControllerBase
    {
        private readonly IIstatService _service;

        public IstatController(IIstatService serviceSession)
        {
            _service = serviceSession;
        }

        [HttpGet]
        public async Task<ActionResult<IstatLastDateResponse>> GetLastDate()
        {
            return Ok(await _service.GetLastDate());
        }

        [HttpPost]
        public async Task<IActionResult> SendNewData(IstatSendDataRequest request)
        {
            await _service.SendNewData(request);
            return Ok();
        }

        [HttpGet("movements")]
        public async Task<ActionResult<IstatMovementsDTO>> GetMovements()
        {
            return Ok(await _service.GetMovements());
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMovements([FromBody] IstatMovementsDTO movements)
        {
            await _service.SendMovements(movements);
            return Ok();
        }

        [HttpGet("countries")]
        public async Task<ActionResult<List<string>>> GetCountries()
        {
            return Ok(await _service.GetCountries());
        }
    }
}
