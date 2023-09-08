using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/floors")]
    [ApiController]
    [Authorize]
    public class FloorsController : ControllerBase
    {
        private readonly IFloorsService _service;

        public FloorsController(IFloorsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Floor>>> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Floor>> Get(long id)
        {
            return Ok(await _service.Get(id));
        }

        [HttpPost]
        public async Task<ActionResult<Floor>> Post(Floor floor)
        {
            var result = await _service.Create(floor);
            return CreatedAtAction("Get", new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Floor>> Put(long id, Floor floor)
        {
            return Ok(await _service.Update(id, floor));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}
