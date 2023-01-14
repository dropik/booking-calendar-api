using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/city-tax")]
    [ApiController]
    public class CityTaxController : ControllerBase
    {
        private readonly ICityTaxService _service;

        public CityTaxController(ICityTaxService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<CityTaxResponse>> Get(string from, string to)
        {
            return Ok(await _service.Get(from, to));
        }
    }
}
