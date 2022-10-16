using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class BookingsExtensions
    {        
        public static IEnumerable<FlattenedRoom> SelectInRangeRooms(this IEnumerable<Booking> bookings, string from, string to)
        {
            var (fromDate, toDate) = GetInitialRange(from, to);
            var rooms = bookings.Flatten().ExcludeByRange(fromDate, toDate);

            // extending search range to fetch all tiles that might possibly collide with
            // tiles that fall in the range, to ensure correct collision detection in front-end
            if (rooms.Any())
            {
                (fromDate, toDate) = GetExtendedRange(rooms);
                rooms = bookings.Flatten().ExcludeByRange(fromDate, toDate);
            }

            return rooms;
        }

        private static IEnumerable<FlattenedRoom> Flatten(this IEnumerable<Booking> bookings)
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

        private static IEnumerable<FlattenedRoom> ExcludeByRange(this IEnumerable<FlattenedRoom> rooms, DateTime fromDate, DateTime toDate)
        {
            return rooms.Where(room => (room.To - fromDate).Days >= 0 && (toDate - room.From).Days >= 0);
        }

        private static (DateTime fromDate, DateTime toDate) GetInitialRange(string from, string to)
        {
            return
            (
                DateTime.ParseExact(from, "yyyy-MM-dd", null),
                DateTime.ParseExact(to, "yyyy-MM-dd", null)
            );
        }

        private static (DateTime fromDate, DateTime toDate) GetExtendedRange(IEnumerable<FlattenedRoom> rooms)
        {
            var firstArrival = rooms.OrderBy(tile => tile.From).First();
            var lastDeparture = rooms.OrderBy(tile => tile.To).Last();

            return
            (
                firstArrival.From.AddDays(1),
                lastDeparture.To.AddDays(-1)
            );
        }
    }
}
