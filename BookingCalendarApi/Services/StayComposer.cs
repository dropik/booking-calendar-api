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
                            .Select(roomContainerGroup => new
                            {
                                StayId = roomContainerGroup.Key,
                                roomContainerGroup.OrderBy(roomContainer => roomContainer.Room.Arrival).First().Room.Arrival,
                                roomContainerGroup.OrderBy(roomContainer => roomContainer.Room.Departure).Last().Room.Departure,
                                roomContainerGroup.First().Room.Guests,
                                roomContainerGroup.First().RoomId
                            })
                    }
                )
                .SelectMany(
                    bookingContainer => bookingContainer.Rooms,
                    (bookingContainer, room) => new Stay(
                        stayId: room.StayId,
                        bookingNumber: bookingContainer.Booking.BookingNumber,
                        arrival: room.Arrival,
                        departure: room.Departure
                    )
                    {
                        Guests = room.Guests,
                        RoomId = room.RoomId
                    }
                );
        }
    }
}
