using BookingCalendarApi.Services;
using System.Threading.Tasks;

#if NET
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Http;
#endif

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/bookings")]
    [Authorize]
    public class BookingsController : ApiController
    {
        private readonly IBookingsService _service;

        public BookingsController(IBookingsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IHttpActionResult> GetByPeriod([FromUri] string from, [FromUri] string to)
        {
            return Ok(await _service.GetByPeriod(from, to));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> Get(string id, [FromUri] string from)
        {
            return Ok(await _service.Get(id, from));
        }

        [HttpGet]
        [Route("by-name")]
        public async Task<IHttpActionResult> GetByName([FromUri] string from, [FromUri] string to, [FromUri] string name)
        {
            return Ok(await _service.GetByName(from, to, name));
        }
    }
}