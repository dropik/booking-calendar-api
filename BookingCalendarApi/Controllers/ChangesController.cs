using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangesController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> PostAsync(IDictionary<string, ChangeDesc> changes)
        {
            return Ok();
        }
    }
}
