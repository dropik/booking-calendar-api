using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Services;
using System.Threading.Tasks;

#if NET
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
#else
using System.Web.Http;
#endif

namespace BookingCalendarApi.Controllers
{
    [Route("/api/v1/assignments")]
    [Authorize]
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
