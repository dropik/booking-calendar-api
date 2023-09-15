using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [RoutePrefix("api/v1/clients")]
    [JwtAuthentication]
    public class ClientsController : ApiController
    {
        private readonly IClientsService _service;

        public ClientsController(IClientsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("by-query")]
        public async Task<IHttpActionResult> GetByQuery(string query, string from, string to)
        {
            return Ok(await _service.GetByQuery(query, from, to));
        }

        [HttpGet]
        [Route("by-tile")]
        public async Task<IHttpActionResult> GetByTile(string bookingId, string tileId)
        {
            return Ok(await _service.GetByTile(bookingId, tileId));
        }
    }
}
