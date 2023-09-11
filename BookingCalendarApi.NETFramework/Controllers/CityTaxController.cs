using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/city-tax")]
    [JwtAuthentication]
    public class CityTaxController : ApiController
    {
        private readonly ICityTaxService _service;

        public CityTaxController(ICityTaxService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IHttpActionResult> Get(string from, string to)
        {
            return Ok(await _service.Get(from, to));
        }
    }
}
