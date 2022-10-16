using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChangesController : ControllerBase
    {
        private readonly BookingCalendarContext _context;
        private readonly IIperbooking _iperbooking;

        public ChangesController(BookingCalendarContext context, IIperbooking iperbooking)
        {
            _context = context;
            _iperbooking = iperbooking;
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

                    var fromDate = DateTime.ParseExact(change.From, "yyyy-MM-dd", null);
                    var arrivalFrom = fromDate.AddDays(-30).ToString("yyyyMMdd");
                    var arrivalTo = fromDate.AddDays(30).ToString("yyyyMMdd");
                    var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);
                    var rooms = bookings.Flatten();

                    if (change.RoomChanged && change.NewRoom != null && rooms.Any())
                    {
                        var hasCollision = false;
                        // todo: collision check
                    }

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
                // rolling back changes on error
                var changedEntries = _context.ChangeTracker.Entries()
                    .Where(e => e.State != EntityState.Unchanged).ToList();

                foreach (var entry in changedEntries)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            entry.CurrentValues.SetValues(entry.OriginalValues);
                            entry.State = EntityState.Unchanged;
                            break;
                        case EntityState.Added:
                            entry.State = EntityState.Detached;
                            break;
                        case EntityState.Deleted:
                            entry.State = EntityState.Unchanged;
                            break;
                    }
                }

                return BadRequest();
            }
        }
    }
}
