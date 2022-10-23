using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class AssignedBookingComposer : IAssignedBookingComposer
    {
        private readonly BookingCalendarContext _context;

        public AssignedBookingComposer(BookingCalendarContext context)
        {
            _context = context;
        }

        public IEnumerable<AssignedBooking> Compose(IEnumerable<Booking> source) =>
            source
            .Select(
                booking => new AssignedBooking(booking)
                {
                    Rooms = booking.Rooms
                        .GroupJoin(
                            _context.RoomAssignments,
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
