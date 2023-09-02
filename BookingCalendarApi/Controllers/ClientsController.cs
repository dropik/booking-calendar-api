using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientsService _service;

        public ClientsController(IClientsService service)
        {
            _service = service;
        }

        [HttpGet("clients-by-query")]
        public async Task<ActionResult<List<ClientWithBookingResponse>>> GetByQuery(string query, string from, string to)
        {
            return Ok(await _service.GetByQuery(query, from, to));
        }

        [HttpGet("clients-by-tile")]
        public async Task<ActionResult<List<ClientResponse>>> GetByTile(string bookingId, string tileId)
        {
            return Ok(await _service.GetByTile(bookingId, tileId));
        }
    }
}
