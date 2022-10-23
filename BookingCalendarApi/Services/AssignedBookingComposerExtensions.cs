using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class AssignedBookingComposerExtensions
    {
        public static IEnumerable<AssignedBooking> ExcludeNotAssigned(this IEnumerable<AssignedBooking> bookings) =>
            bookings
            .Select(booking => new AssignedBooking(booking.Booking)
            {
                Rooms = booking.Rooms
                    .Where(room => room.RoomId != null)
            })
            .Where(booking => booking.Rooms.Any());
    }
}
