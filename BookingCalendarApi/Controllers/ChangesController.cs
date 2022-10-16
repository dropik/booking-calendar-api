using BookingCalendarApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangesController : ControllerBase
    {
        private readonly BookingCalendarContext _context;

        public ChangesController(BookingCalendarContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(IDictionary<string, ChangeDesc> changes)
        {
            if (changes == null)
            {
                return Ok();
            }

            try
            {
                foreach (var (tileId, change) in changes)
                {
                    var assignment = await _context.TileAssignments.SingleOrDefaultAsync(a => a.Id == tileId);
                    if (assignment != null && assignment.Id != string.Empty)
                    {
                        if (change.NewColor != null)
                        {
                            assignment.Color = change.NewColor;
                        }
                    }
                    else
                    {
                        if (change.NewColor != null)
                        {
                            _context.TileAssignments.Add(new TileAssignment(tileId, change.NewColor));
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
