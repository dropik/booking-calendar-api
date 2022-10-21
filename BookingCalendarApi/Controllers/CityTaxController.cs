using BookingCalendarApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/city-tax")]
    public class CityTaxController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<CityTax>> GetAsync(string from, string to)
        {
            return Ok();
        }
    }
}
