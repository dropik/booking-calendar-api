using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class StayComposer : IStayComposer
    {
        public IEnumerable<Stay> Compose(IEnumerable<AssignedBooking> source) =>
            source
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
