using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
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
                var random = new Random();
                Dictionary<string, AssignedTileDesc> assignedTiles = new();

                foreach (var (tileId, change) in changes)
                {
                    var assignmentForChange = await _context.TileAssignments.SingleOrDefaultAsync(a => a.Id == tileId);

                    var fromDate = DateTime.ParseExact(change.From, "yyyy-MM-dd", null);
                    var toDate = DateTime.ParseExact(change.To, "yyyy-MM-dd", null);
                    var arrivalFrom = fromDate.AddDays(-30).ToString("yyyyMMdd");
                    var arrivalTo = toDate.AddDays(30).ToString("yyyyMMdd");
                    var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);
                    var rooms = bookings.Flatten();

                    foreach (var room in rooms)
                    {
                        if (!assignedTiles.ContainsKey(room.Id))
                        {
                            var assignmentForRoom = await _context.TileAssignments.SingleOrDefaultAsync(a => a.Id == room.Id);
                            if (
                                assignmentForRoom != null &&
                                assignmentForRoom.Id == room.Id &&
                                assignmentForRoom.RoomId != null &&
                                room.Booking.Status != BookingStatus.Cancelled
                            )
                            {
                                assignedTiles.Add(room.Id, new AssignedTileDesc(room.From, room.To, (long)assignmentForRoom.RoomId));
                            }
                        }
                    }

                    if (change.RoomChanged && change.NewRoom != null)
                    {
                        foreach (var (assignedTileId, assignedTile) in assignedTiles)
                        {
                            if (change.NewRoom == assignedTile.RoomId &&
                                (((assignedTile.Arrival - fromDate).Days >= 0 && (assignedTile.Arrival - toDate).Days < 0) ||
                                ((assignedTile.Departure - fromDate).Days > 0 && (assignedTile.Departure - toDate).Days <= 0)))
                            {
                                throw new Exception($"Collision between tiles {tileId} and {assignedTileId} detected on room id {assignedTile.RoomId}. Reverting...");
                            }
                        }
                        if (!assignedTiles.ContainsKey(tileId))
                        {
                            assignedTiles.Add(tileId, new AssignedTileDesc(fromDate, toDate, (long)change.NewRoom));
                        }
                        else
                        {
                            assignedTiles[tileId].RoomId = (long)change.NewRoom;
                        }
                    }

                    if (change.NewRoom == null)
                    {
                        assignedTiles.Remove(tileId);
                    }

                    if (assignmentForChange != null && assignmentForChange.Id != string.Empty)
                    {
                        if (change.NewColor != null)
                        {
                            assignmentForChange.Color = change.NewColor;
                        }
                        assignmentForChange.RoomId = change.NewRoom;
                    }
                    else
                    {
                        var color = change.NewColor ?? $"booking{(random.Next() % 8) + 1}";
                        _context.TileAssignments.Add(new TileAssignment(tileId, color) { RoomId = change.NewRoom });
                    }
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
