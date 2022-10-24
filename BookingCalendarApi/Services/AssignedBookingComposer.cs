using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class AssignedBookingComposer : IAssignedBookingComposer
    {
        private readonly BookingCalendarContext _context;
        private readonly IEnumerable<RoomAssignment> _assignments;

        public AssignedBookingComposer(BookingCalendarContext context)
        {
            _context = context;
            _assignments = context.RoomAssignments;
        }

        public AssignedBookingComposer(BookingCalendarContext context, IEnumerable<RoomAssignment> assignments)
        {
            _context = context;
            _assignments = assignments;
        }

        public IEnumerable<AssignedBooking<Guest>> Compose(IEnumerable<Models.Iperbooking.Bookings.Booking> source) =>
            source
            .Select(
                booking => new AssignedBooking<Guest>(booking)
                {
                    Rooms = booking.Rooms
                        .GroupJoin(
                            _assignments,
                            room => $"{room.StayId}-{room.Arrival}-{room.Departure}",
                            assignment => assignment.Id,
                            (room, assignments) => new { Room = room, Assignments = assignments }
                        )
                        .SelectMany(
                            join => join.Assignments.DefaultIfEmpty(),
                            (join, assignment) => new { join.Room, assignment?.RoomId }
                        )
                        .GroupBy(roomContainer => roomContainer.Room.StayId)
                        .Select(roomContainerGroup => new AssignedRoom<Guest>(
                            stayId: roomContainerGroup.Key,
                            roomName: roomContainerGroup.First().Room.RoomName,
                            arrival: roomContainerGroup.OrderBy(roomContainer => roomContainer.Room.Arrival).First().Room.Arrival,
                            departure: roomContainerGroup.OrderBy(roomContainer => roomContainer.Room.Departure).Last().Room.Departure
                        )
                        {
                            Guests = roomContainerGroup.First().Room.Guests,
                            RoomId = roomContainerGroup.First().RoomId
                        })
                }
            );
    }
}
