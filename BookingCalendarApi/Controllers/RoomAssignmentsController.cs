using BookingCalendarApi.Controllers.Internal;
using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/room-assignments")]
    [ApiController]
    public class RoomAssignmentsController : ControllerBase
    {
        private readonly BookingCalendarContext _context;
        private readonly IIperbooking _iperbooking;

        public RoomAssignmentsController(BookingCalendarContext context, IIperbooking iperbooking)
        {
            _context = context;
            _iperbooking = iperbooking;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(IDictionary<string, long?> assignments)
        {
            if (assignments == null)
            {
                return Ok();
            }

            try
            {
                var random = new Random();
                Dictionary<string, AssignedTileDesc> assignedTiles = new();

                var assignmentPeriods = assignments.Select(assignment =>
                {
                    var tileIdSplit = assignment.Key.Split("-");
                    return new
                    {
                        From = tileIdSplit[1],
                        To = tileIdSplit[2]
                    };
                });
                var firstArrival = assignmentPeriods.OrderBy(p => p.From).First().From;
                var lastDeparture = assignmentPeriods.OrderBy(p => p.To).Last().To;

                var arrivalFrom = DateTime.ParseExact(firstArrival, "yyyyMMdd", null).AddDays(-20).ToString("yyyyMMdd");
                var arrivalTo = DateTime.ParseExact(lastDeparture, "yyyyMMdd", null).AddDays(20).ToString("yyyyMMdd");
                var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);
                var flattenedBookings = bookings.SelectMany(booking => booking.Rooms, (booking, room) => new { booking, room });

                foreach (var flattenedBooking in flattenedBookings)
                {
                    var id = $"{flattenedBooking.room.StayId}-{flattenedBooking.room.Arrival}-{flattenedBooking.room.Departure}";

                    if (!assignedTiles.ContainsKey(id))
                    {
                        var assignmentForRoom = await _context.RoomAssignments.SingleOrDefaultAsync(a => a.Id == id);
                        if (
                            assignmentForRoom != null &&
                            assignmentForRoom.Id == id &&
                            assignmentForRoom.RoomId != null &&
                            flattenedBooking.booking.Status != BookingStatus.Cancelled
                        )
                        {
                            assignedTiles.Add(id, new AssignedTileDesc(
                                DateTime.ParseExact(flattenedBooking.room.Arrival, "yyyyMMdd", null),
                                DateTime.ParseExact(flattenedBooking.room.Departure, "yyyyMMdd", null),
                                (long)assignmentForRoom.RoomId)
                            );
                        }
                    }
                }

                foreach (var (tileId, proposedNewRoomId) in assignments)
                {
                    var assignmentForTile = await _context.RoomAssignments.SingleOrDefaultAsync(a => a.Id == tileId);

                    var tileIdSplit = tileId.Split("-");
                    var fromDate = DateTime.ParseExact(tileIdSplit[1], "yyyyMMdd", null);
                    var toDate = DateTime.ParseExact(tileIdSplit[2], "yyyyMMdd", null);

                    var bookingStatusOfAssignment = flattenedBookings
                        .Where(booking => booking.room.StayId.ToString() == tileIdSplit[0])
                        .First().booking.Status;

                    var newRoomId = (bookingStatusOfAssignment == BookingStatus.Cancelled) ? null : proposedNewRoomId;

                    if (newRoomId != null)
                    {
                        foreach (var (assignedTileId, assignedTile) in assignedTiles)
                        {
                            if (assignedTileId != tileId &&
                                newRoomId == assignedTile.RoomId &&
                                (((assignedTile.Arrival - fromDate).Days >= 0 && (assignedTile.Arrival - toDate).Days < 0) ||
                                ((assignedTile.Departure - fromDate).Days > 0 && (assignedTile.Departure - toDate).Days <= 0)))
                            {
                                throw new Exception($"Collision between tiles {tileId} and {assignedTileId} detected on room id {assignedTile.RoomId}. Reverting...");
                            }
                        }
                        if (!assignedTiles.ContainsKey(tileId))
                        {
                            assignedTiles.Add(tileId, new AssignedTileDesc(fromDate, toDate, (long)newRoomId));
                        }
                        else
                        {
                            assignedTiles[tileId].RoomId = (long)newRoomId;
                        }
                    }
                    else
                    {
                        assignedTiles.Remove(tileId);
                    }

                    if (assignmentForTile != null && assignmentForTile.Id != string.Empty)
                    {
                        assignmentForTile.RoomId = newRoomId;
                    }
                    else
                    {
                        _context.RoomAssignments.Add(new RoomAssignment(tileId) { RoomId = newRoomId });
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
