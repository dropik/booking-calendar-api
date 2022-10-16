using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class RoomsExtensions
    {
        public static IEnumerable<FlattenedRoom> Flatten(this IEnumerable<Booking> bookings)
        {
            return bookings
               .SelectMany(
                   booking => booking.Rooms,
                   (booking, room) => new FlattenedRoom(
                       $"{room.StayId}-{room.Arrival}-{room.Departure}",
                       DateTime.ParseExact(room.Arrival, "yyyyMMdd", null),
                       DateTime.ParseExact(room.Departure, "yyyyMMdd", null),
                       booking,
                       room
                   ));
        }

        public static IEnumerable<FlattenedRoom> ExcludeByRange(this IEnumerable<FlattenedRoom> rooms, DateTime fromDate, DateTime toDate)
        {
            return rooms.Where(room => (room.To - fromDate).Days >= 0 && (toDate - room.From).Days >= 0);
        }
    }
}
