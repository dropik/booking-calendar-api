using BookingCalendarApi.Exceptions;
using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
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

            var bookings = await GetBookings(assignmentRequests);

            // eliminate all cancelled bookings
            var cancelledIds = bookings
                .Where(booking => booking.Status == BookingStatus.Cancelled)
                .SelectMany(booking => booking.Rooms, (booking, room) => $"{room.StayId}-{room.Arrival}-{room.Departure}")
                .ToList();

            var cancelledAssignments = await (from assignment in _context.RoomAssignments
                                              where cancelledIds.Contains(assignment.Id)
                                              select assignment)
                                             .ToListAsync();

            foreach ( var assignment in cancelledAssignments )
            {
                _context.Remove(assignment);
            }
            await _context.SaveChangesAsync();

            // compiling current ocupations table
            var nonCancelledIds = bookings
                .Where(booking => booking.Status != BookingStatus.Cancelled)
                .SelectMany(booking => booking.Rooms, (booking, room) => $"{room.StayId}-{room.Arrival}-{room.Departure}")
                .ToList();

            var currentAssignments = await (from assignment in _context.RoomAssignments
                                            where nonCancelledIds.Contains(assignment.Id)
                                            select assignment)
                                           .ToListAsync();

            var roomOccupations = currentAssignments
                .Join(bookings
                    .Where(booking => booking.Status != BookingStatus.Cancelled)
                    .SelectMany(booking => booking.Rooms, (booking, room) => new { Id = $"{room.StayId}-{room.Arrival}-{room.Departure}", room.Arrival, room.Departure }),
                    assignment => assignment.Id,
                    room => room.Id,
                    (assignment, room) => new { assignment.Id, assignment.RoomId, room.Arrival, room.Departure })
                .ToDictionary(join => join.Id);

            // applying new assignments to table
            foreach (var (tileId, roomId) in assignmentRequests)
            {
                if (roomOccupations.TryGetValue(tileId, out var value))
                {
                    if (roomId == null)
                    {
                        roomOccupations.Remove(tileId);
                    }
                    else
                    {
                        roomOccupations[tileId] = new { Id = tileId, RoomId = roomId.Value, value.Arrival, value.Departure };
                    }
                }
                else
                {
                    if (roomId == null)
                    {
                        continue;
                    }

                    var tileIdSplit = tileId.Split("-");
                    roomOccupations.Add(tileId, new { Id = tileId, RoomId = roomId.Value, Arrival = tileIdSplit[1], Departure = tileIdSplit[2] });
                }
            }

            // checking for collisions
            var occupationRoomGroups = roomOccupations
                .OrderBy(occupation => occupation.Value.Arrival)
                .GroupBy(occupation => occupation.Value.RoomId, (item) => new { Id = item.Key, item.Value.Arrival, item.Value.Departure });

            foreach (var group in occupationRoomGroups)
            {
                var currentItem = group.First();
                foreach (var nextItem in group.Skip(1))
                {
                    var currentDeparture = DateTime.ParseExact(currentItem.Departure, "yyyyMMdd", null);
                    var nextArrival = DateTime.ParseExact(nextItem.Arrival, "yyyyMMdd", null);
                    if (nextArrival < currentDeparture)
                    {
                        throw new BookingCalendarException(BCError.ROOMS_COLLISION, $"Collision between tiles {currentItem.Id} and {nextItem.Id} detected on room id {group.Key}. Reverting...");
                    }
                }
            }

            // saving assignments
            var requestedIds = assignmentRequests.Select(r => r.Key).ToList();
            var modifiedAssignments = await (from assignment in _context.RoomAssignments
                                             where requestedIds.Contains(assignment.Id)
                                             select assignment
                                            ).ToDictionaryAsync(assignment => assignment.Id);

            foreach (var (tileId, roomId) in assignmentRequests)
            {
                if (modifiedAssignments.TryGetValue(tileId, out var assignment))
                {
                    if (roomId == null)
                    {
                        _context.Remove(assignment);
                    }
                    else
                    {
                        assignment.RoomId = roomId.Value;
                        _context.Attach(assignment);
                        _context.Entry(assignment).State = EntityState.Modified;
                    }
                }
                else
                {
                    if (roomId == null)
                    {
                        continue;
                    }
                    assignment = new() { Id = tileId, RoomId = roomId.Value };
                    _context.Add(assignment);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task<IEnumerable<Booking>> GetBookings(IDictionary<string, long?> assignmentRequests)
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
    }
}
