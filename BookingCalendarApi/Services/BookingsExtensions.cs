using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public static class BookingsExtensions
    {        
        public static IEnumerable<Booking> SelectInRange(this IEnumerable<Booking> bookings, string from, string to, bool autoExtend = false)
        {
            var (fromDate, toDate) = GetInitialRange(from, to);
            var rooms = bookings.ExcludeByRange(fromDate, toDate);

            // extending search range to fetch all tiles that might possibly collide with
            // tiles that fall in the range, to ensure correct collision detection in front-end
            if (autoExtend && rooms.Any())
            {
                (fromDate, toDate) = GetExtendedRange(rooms);
                rooms = bookings.ExcludeByRange(fromDate, toDate);
            }

            return rooms;
        }

        public static IEnumerable<Booking> SelectByName(this IEnumerable<Booking> bookings, string name)
        {
            return bookings
                .Where(booking => $"{booking.FirstName} {booking.LastName}".Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<Booking> SelectById(this IEnumerable<Booking> bookings, string id)
        {
            return bookings
                .Where(booking => booking.BookingNumber.ToString() == id);
        }

        private static IEnumerable<Booking> ExcludeByRange(this IEnumerable<Booking> bookings, DateTime fromDate, DateTime toDate)
        {
            return bookings
                .Where(booking => booking.Rooms
                    .Where(room =>
                        (DateTime.ParseExact(room.Departure, "yyyyMMdd", null) - fromDate).Days >= 0 &&
                        (toDate - DateTime.ParseExact(room.Arrival, "yyyyMMdd", null)).Days >= 0)
                    .Any());
        }

        private static (DateTime fromDate, DateTime toDate) GetInitialRange(string from, string to)
        {
            return
            (
                DateTime.ParseExact(from, "yyyy-MM-dd", null),
                DateTime.ParseExact(to, "yyyy-MM-dd", null)
            );
        }

        private static (DateTime fromDate, DateTime toDate) GetExtendedRange(IEnumerable<Booking> bookings)
        {
            var rooms = bookings.SelectMany(booking => booking.Rooms, (booking, room) => room);
            var firstArrival = rooms.OrderBy(tile => tile.Arrival).First();
            var lastDeparture = rooms.OrderBy(tile => tile.Departure).Last();

            return
            (
                DateTime.ParseExact(firstArrival.Arrival, "yyyyMMdd", null).AddDays(1),
                DateTime.ParseExact(lastDeparture.Departure, "yyyyMMdd", null).AddDays(-1)
            );
        }
    }
}
