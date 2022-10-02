using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TilesController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;

        public TilesController(IIperbooking iperbooking)
        {
            _iperbooking = iperbooking;
        }
    }
}