using BookingCalendarApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[color-assignments]")]
    [ApiController]
    public class ColorAssignmentsController : ControllerBase
    {
        private readonly BookingCalendarContext _context;

        public ColorAssignmentsController(BookingCalendarContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(IDictionary<string, string> colors)
        {
            if (colors == null)
            {
                return BadRequest();
            }

            try
            {
                foreach (var (bookingId, color) in colors)
                {
                    await StoreAssignmentAsync(bookingId, color);
                }

                await _context.SaveChangesAsync();

                return Ok();
            } catch (Exception)
            {
                return BadRequest();
            }
        }

        private async Task StoreAssignmentAsync(string bookingId, string color)
        {
            var assignment = await _context.ColorAssignments.SingleOrDefaultAsync(a => a.BookingId == bookingId);
            if (assignment != null && assignment.BookingId == bookingId)
            {
                assignment.Color = color;
            }
            else
            {
                _context.ColorAssignments.Add(new ColorAssignment(bookingId, color));
            }
        }
    }
}