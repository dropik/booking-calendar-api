using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [RoutePrefix("api/v1/assignments")]
    [JwtAuthentication]
    public class AssignmentsController : ApiController
    {
        private readonly IAssignmentsService _service;

        public AssignmentsController(IAssignmentsService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IHttpActionResult> SetAssignments([FromBody] AssignmentsRequest request)
        {
            await _service.Set(request);
            return Ok();
        }
    }
}
