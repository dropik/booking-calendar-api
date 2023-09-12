using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [RoutePrefix("api/v1/rooms")]
    [JwtAuthentication]
    public class RoomsController : ApiController
    {
        private readonly IRoomsService _service;

        public RoomsController(IRoomsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IHttpActionResult> Get(long id)
        {
            return Ok(await _service.Get(id));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(Room room)
        {
            var result = await _service.Create(room);
            return CreatedAtRoute("api/v1/rooms/{id}", new { id = result.Id }, result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(long id, Room room)
        {
            return Ok(await _service.Update(id, room));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(long id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}
