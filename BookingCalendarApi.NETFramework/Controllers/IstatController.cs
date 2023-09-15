using BookingCalendarApi.Models.DTO;
using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [RoutePrefix("api/v1/istat")]
    [JwtAuthentication]
    public class IstatController : ApiController
    {
        private readonly IIstatService _service;

        public IstatController(IIstatService serviceSession)
        {
            _service = serviceSession;
        }

        [HttpGet]
        [Route("movements")]
        public async Task<IHttpActionResult> GetMovements()
        {
            return Ok(await _service.GetMovements());
        }

        [HttpPost]
        [Route("movements")]
        public async Task<IHttpActionResult> SendMovements([FromBody] IstatMovementsDTO movements)
        {
            await _service.SendMovements(movements);
            return Ok();
        }

        [HttpGet]
        [Route("countries")]
        public async Task<IHttpActionResult> GetCountries()
        {
            return Ok(await _service.GetCountries());
        }
    }
}
