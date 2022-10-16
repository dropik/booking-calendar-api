using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingCalendarApi.Models;
using BookingCalendarApi.Services;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly BookingCalendarContext _context;
        private readonly IIperbooking _iperbooking;

        public RoomsController(BookingCalendarContext context, IIperbooking iperbooking)
        {
            _context = context;
            _iperbooking = iperbooking;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRoomsAsync()
        {
            return await _context.Rooms.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Room>> GetRoomAsync(long id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            return room;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoomAsync(long id, Room room)
        {
            if (id != room.Id)
            {
                return BadRequest();
            }

            if (!(await RoomTypeExistsAsync(room.Type)))
            {
                return BadRequest("Given room type was not found on Iperbooking");
            }

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoomExists(id))
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
        public async Task<ActionResult<Room>> PostRoomAsync(Room room)
        {
            if (!(await RoomTypeExistsAsync(room.Type)))
            {
                return BadRequest("Given room type was not found on Iperbooking");
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = room.Id }, room);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoomAsync(long id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoomExists(long id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }

        private async Task<bool> RoomTypeExistsAsync(string roomType)
        {
            var roomRates = await _iperbooking.GetRoomRatesAsync();
            var matchedTypes = roomRates.Where(r => r.RoomName == roomType);

            return matchedTypes.Any();
        }
    }
}
