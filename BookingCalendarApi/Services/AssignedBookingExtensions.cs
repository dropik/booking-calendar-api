using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class AssignedBookingExtensions
    {
        public static IEnumerable<AssignedBooking<Guest>> ExcludeNotAssigned(this IEnumerable<AssignedBooking<Guest>> bookings) =>
            bookings
            .Select(booking => new AssignedBooking<Guest>(booking.Booking)
            {
                Rooms = booking.Rooms
                    .Where(room => room.RoomId != null)
            })
            .Where(booking => booking.Rooms.Any());
    }
}
