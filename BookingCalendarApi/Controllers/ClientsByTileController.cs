using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/clients-by-tile")]
    [ApiController]
    public class ClientsByTileController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;

        public ClientsByTileController(IIperbooking iperbooking)
        {
            _iperbooking = iperbooking;
        }
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAsync(string bookingId, string tileId)
        {
            try
            {
                var stayId = tileId.Split("-")[0];
                var response = await _iperbooking.GetGuestsAsync(bookingId);

                return response.Reservations
                    .SelectByStayId(stayId)
                    .ComposeResponse()
                    .ToList();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}