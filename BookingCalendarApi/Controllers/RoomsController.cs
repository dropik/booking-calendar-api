using Microsoft.AspNetCore.Mvc;
using BookingCalendarApi.Services;
using BookingCalendarApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/rooms")]
    [ApiController]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomsService _service;

        public RoomsController(IRoomsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Room>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> Get(long id)
        {
            return Ok(await _service.Get(id));
        }

        [HttpPost]
        public async Task<ActionResult<Room>> Post(Room room)
        {
            var result = await _service.Create(room);
            return CreatedAtAction("Get", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Room>> Put(long id, Room room)
        {
            return Ok(await _service.Update(id, room));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}
