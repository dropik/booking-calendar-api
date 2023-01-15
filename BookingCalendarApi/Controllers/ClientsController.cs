using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _service;

        public ClientsController(IClientsService service)
        {
            _service = service;
        }

        [HttpGet("clients-by-query")]
        public async Task<ActionResult<IEnumerable<ClientWithBooking>>> GetByQuery(string query, string from, string to)
        {
            return Ok(await _service.GetByQuery(query, from, to));
        }
    }
}
