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
    [Route("api/v1/city-tax")]
    [Authorize]
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
