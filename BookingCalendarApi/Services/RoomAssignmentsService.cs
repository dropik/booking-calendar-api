using BookingCalendarApi.Models.Internal;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class RoomAssignmentsService : IRoomAssignmentsService
    {
        private readonly BookingCalendarContext _context;
        private readonly IIperbooking _iperbooking;

        public RoomAssignmentsService(BookingCalendarContext context, IIperbooking iperbooking)
        {
            _context = context;
            _iperbooking = iperbooking;
        }

        public async Task AssignRooms(IDictionary<string, long?> assignmentRequests)
        {
            if (assignmentRequests == null)
            {
                return;
            }

            var bookings = await GetBookingsAsync(assignmentRequests);
            var assignedTiles = await CreateAssignedTilesCacheAsync(bookings);

            foreach (var (tileId, proposedNewRoomId) in assignmentRequests)
            {
                await StoreAssignmentAsync(tileId, proposedNewRoomId, bookings, assignedTiles);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<IEnumerable<Booking>> GetBookingsAsync(IDictionary<string, long?> assignmentRequests)
        {
            var assignmentPeriods = assignmentRequests.Select(assignmentRequest =>
            {
                var tileIdSplit = assignmentRequest.Key.Split("-");
                return new
                {
                    From = tileIdSplit[1],
                    To = tileIdSplit[2]
                };
            });
            var firstArrival = assignmentPeriods.OrderBy(p => p.From).First().From;
            var lastDeparture = assignmentPeriods.OrderBy(p => p.To).Last().To;

            var arrivalFrom = DateTime.ParseExact(firstArrival, "yyyyMMdd", null).ToString("yyyy-MM-dd");
            var arrivalTo = DateTime.ParseExact(lastDeparture, "yyyyMMdd", null).ToString("yyyy-MM-dd");
            return await _iperbooking.GetBookings(arrivalFrom, arrivalTo);
        }

        private async Task<IDictionary<string, AssignedTile>> CreateAssignedTilesCacheAsync(IEnumerable<Booking> bookings)
        {
            Dictionary<string, AssignedTile> assignedTiles = new();

            foreach (var booking in bookings.SelectMany(booking => booking.Rooms, (booking, room) => new { booking, room }))
            {
                var id = $"{booking.room.StayId}-{booking.room.Arrival}-{booking.room.Departure}";

                var assignment = await _context.RoomAssignments.SingleOrDefaultAsync(a => a.Id == id);
                if (
                    assignment != null &&
                    assignment.Id == id &&
                    assignment.RoomId != null &&
                    booking.booking.Status != BookingStatus.Cancelled
                )
                {
                    assignedTiles.Add(id, new AssignedTile(
                        DateTime.ParseExact(booking.room.Arrival, "yyyyMMdd", null),
                        DateTime.ParseExact(booking.room.Departure, "yyyyMMdd", null),
                        (long)assignment.RoomId)
                    );
                }
            }

            return assignedTiles;
        }

        private async Task StoreAssignmentAsync(string tileId, long? proposedNewRoomId, IEnumerable<Booking> bookings, IDictionary<string, AssignedTile> assignedTiles)
        {
            var assignment = await _context.RoomAssignments.SingleOrDefaultAsync(a => a.Id == tileId);

            var tileIdSplit = tileId.Split("-");
            var fromDate = DateTime.ParseExact(tileIdSplit[1], "yyyyMMdd", null);
            var toDate = DateTime.ParseExact(tileIdSplit[2], "yyyyMMdd", null);

            var bookingStatusOfAssignment = bookings
                .Where(booking => booking.Rooms.Any(room => room.StayId.ToString() == tileIdSplit[0]))
                .First().Status;

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
                    assignedTiles.Add(tileId, new AssignedTile(fromDate, toDate, (long)newRoomId));
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

            if (assignment != null && assignment.Id != string.Empty)
            {
                assignment.RoomId = newRoomId;
            }
            else
            {
                _context.RoomAssignments.Add(new RoomAssignment(tileId) { RoomId = newRoomId });
            }
        }
    }
}
