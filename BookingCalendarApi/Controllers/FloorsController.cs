using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingCalendarApi.Models;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FloorsController : ControllerBase
    {
        private readonly BookingCalendarContext _context;

        public FloorsController(BookingCalendarContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Floor>>> GetFloorsAsync()
        {
            await _context.Floors
                .Include(floor => floor.Rooms
                    .OrderBy(room => room.Id))
                .ToListAsync();
            return await _context.Floors.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Floor>> GetFloorAsync(long id)
        {
            var floor = await _context.Floors.FindAsync(id);

            if (floor == null)
            {
                return NotFound();
            }

            return floor;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutFloorAsync(long id, Floor floor)
        {
            if (id != floor.Id)
            {
                return BadRequest();
            }

            _context.Entry(floor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FloorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Floor>> PostFloorAsync(Floor floor)
        {
            _context.Floors.Add(floor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFloor", new { id = floor.Id }, floor);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFloorAsync(long id)
        {
            var floor = await _context.Floors.FindAsync(id);
            if (floor == null)
            {
                return NotFound();
            }

            _context.Floors.Remove(floor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FloorExists(long id)
        {
            return _context.Floors.Any(e => e.Id == id);
        }
    }
}
