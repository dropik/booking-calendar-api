using BookingCalendarApi.Repository;
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
    [Route("api/v1/rooms")]
    [Authorize]
    public class RoomsController : ApiController
    {
        private readonly IRoomsService _service;

        public RoomsController(IRoomsService service)
        {
            _service = service;
        }

        [HttpGet]
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
