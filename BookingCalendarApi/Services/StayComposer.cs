using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class StayComposer : IStayComposer
    {
        private readonly BookingCalendarContext _context;

        public StayComposer(BookingCalendarContext context)
        {
            _context = context;
        }

        public IEnumerable<Stay> Compose(IEnumerable<Booking> source)
        {
            return source
                .Select(
                    booking => new
                    {
                        Booking = booking,
                        RoomContainers = booking.Rooms
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
                    }
                )
                .SelectMany(
                    bookingContainer => bookingContainer.RoomContainers,
                    (bookingContainer, roomContainer) => new Stay(
                        stayId: roomContainer.Room.StayId,
                        bookingNumber: bookingContainer.Booking.BookingNumber,
                        arrival: roomContainer.Room.Arrival,
                        departure: roomContainer.Room.Departure
                    )
                    {
                        Guests = roomContainer.Room.Guests,
                        RoomId = roomContainer.RoomId
                    }
                );
        }
    }
}
