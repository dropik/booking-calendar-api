using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.NETFramework.Controllers
{
    [RoutePrefix("api/v1/structures")]
    [JwtAuthentication]
    public class StructuresController : ApiController
    {
        private readonly IStructureService _service;

        public StructuresController(IStructureService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("current/api-keys")]
        public async Task<IHttpActionResult> GetApiKeys()
        {
            return Ok(await _service.GetApiKeys());
        }

        [HttpPatch]
        [Route("current/api-keys")]
        public async Task<IHttpActionResult> UpdateApiKeys([FromBody] UpdateAPIKeysRequest request)
        {
            await _service.UpdateApiKeys(request);
            return Ok();
        }
    }
}