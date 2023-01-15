using BookingCalendarApi.Models.Entities;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class AssignedBookingComposer : IAssignedBookingComposer
    {
        private readonly List<RoomAssignment> _assignments;

        public AssignedBookingComposer(DataContext dataContext)
        {
            _assignments = dataContext.RoomAssignments;
        }

        public IEnumerable<AssignedBooking<BookingGuest>> Compose(IEnumerable<Booking> source) =>
            source
            .Select(
                booking => new AssignedBooking<BookingGuest>(booking)
                {
                    Rooms = booking.Rooms
                        .GroupJoin(
                            _assignments,
                            room => $"{room.StayId}-{room.Arrival}-{room.Departure}",
                            assignment => assignment.Id,
                            (room, assignments) => new { Room = room, Assignments = assignments })
                        .SelectMany(
                            join => join.Assignments.DefaultIfEmpty(),
                            (join, assignment) => new { join.Room, assignment?.RoomId })
                        .GroupBy(roomContainer => roomContainer.Room.StayId)
                        .Select(roomContainerGroup => new AssignedRoom<BookingGuest>(
                            stayId: roomContainerGroup.Key,
                            roomName: roomContainerGroup.First().Room.RoomName,
                            arrival: roomContainerGroup.OrderBy(roomContainer => roomContainer.Room.Arrival).First().Room.Arrival,
                            departure: roomContainerGroup.OrderBy(roomContainer => roomContainer.Room.Departure).Last().Room.Departure,
                            rateId: roomContainerGroup.First().Room.RateId)
                        {
                            Guests = roomContainerGroup.First().Room.Guests,
                            RoomId = roomContainerGroup.First().RoomId
                        })
                        .ToList()
                }
            );
    }
}
