using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Repository;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class AssignmentsService : IAssignmentsService
    {
        private readonly IRepository _repository;
        private readonly IIperbooking _iperbooking;

        public AssignmentsService(IRepository repository, IIperbooking iperbooking)
        {
            _repository = repository;
            _iperbooking = iperbooking;
        }

        public async Task Set(AssignmentsRequest request)
        {
            if (request == null)
            {
                return;
            }

            if (request.Colors == null)
            {
                request.Colors = new Dictionary<string, string>();
            }
            if (request.Rooms == null)
            {
                request.Rooms = new Dictionary<string, long?>();
            }

            await AddColors(request.Colors);
            await AddRooms(request.Rooms);

            await _repository.SaveChangesAsync();
        }

        private async Task AddColors(IDictionary<string, string> colors)
        {
            if (colors == null || colors.Count == 0)
            {
                return;
            }

            foreach (var bookingColorPair in colors)
            {
                var bookingId = bookingColorPair.Key;
                var color = bookingColorPair.Value;
                var assignment = await _repository.SingleOrDefaultAsync(_repository.ColorAssignments.Where(a => a.BookingId == bookingId));
                if (assignment != null && assignment.BookingId == bookingId)
                {
                    assignment.Color = color;
                }
                else
                {
                    _repository.Add(new ColorAssignment() { BookingId = bookingId, Color = color });
                }
            }
        }

        private async Task AddRooms(IDictionary<string, long?> rooms)
        {
            if (rooms == null || rooms.Count == 0)
            {
                return;
            }

            var bookings = await GetBookings(rooms);

            var statuses = bookings
                .SelectMany(booking => booking.Rooms, (booking, room) => new { Id = $"{room.StayId}-{room.Arrival}-{room.Departure}", booking.Status })
                .ToDictionary(x => x.Id, x => x.Status);

            var ids = statuses.Select(s => s.Key).ToList();

            var assignments = await _repository.ToListAsync(from assignment in _repository.RoomAssignments
                                                           where ids.Contains(assignment.Id)
                                                           select assignment);

            // removing assignments of bookings that were cancelled
            for (var i = 0; i < assignments.Count; i++)
            {
                var assignment = assignments[i];
                if (statuses[assignment.Id] == BookingStatus.Cancelled)
                {
                    assignments.RemoveAt(i);
                    _repository.Remove(assignment);
                    i--;
                }
            }

            var roomOccupations = assignments
                .Join(bookings
                    .Where(booking => booking.Status != BookingStatus.Cancelled)
                    .SelectMany(booking => booking.Rooms, (booking, room) => new { Id = $"{room.StayId}-{room.Arrival}-{room.Departure}", room.Arrival, room.Departure }),
                    assignment => assignment.Id,
                    room => room.Id,
                    (assignment, room) => new { assignment.Id, assignment.RoomId, room.Arrival, room.Departure })
                .ToDictionary(join => join.Id);

            // applying new assignments to table
            foreach (var tileRoomPair in rooms)
            {
                var tileId = tileRoomPair.Key;
                var roomId = tileRoomPair.Value;
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

                    var tileIdSplit = tileId.Split('-');
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
            var requestedIds = rooms.Select(r => r.Key).ToList();
            var modifiedAssignments = await _repository.ToDictionaryAsync(from assignment in _repository.RoomAssignments
                                                                          where requestedIds.Contains(assignment.Id)
                                                                          select assignment,
                                                                          assignment => assignment.Id);

            foreach (var tileRoomPair in rooms)
            {
                var tileId = tileRoomPair.Key;
                var roomId = tileRoomPair.Value;
                if (modifiedAssignments.TryGetValue(tileId, out var assignment))
                {
                    if (roomId == null)
                    {
                        _repository.Remove(assignment);
                    }
                    else
                    {
                        assignment.RoomId = roomId.Value;
                        _repository.Update(assignment);
                    }
                }
                else
                {
                    if (roomId == null)
                    {
                        continue;
                    }
                    assignment = new RoomAssignment() { Id = tileId, RoomId = roomId.Value };
                    _repository.Add(assignment);
                }
            }
        }

        private async Task<IEnumerable<Booking>> GetBookings(IDictionary<string, long?> assignmentRequests)
        {
            var assignmentPeriods = assignmentRequests.Select(assignmentRequest =>
            {
                var tileIdSplit = assignmentRequest.Key.Split('-');
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
