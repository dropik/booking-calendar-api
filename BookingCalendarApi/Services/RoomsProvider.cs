namespace BookingCalendarApi.Services
{
    public class RoomsProvider : IRoomsProvider
    {
        private readonly IIperbooking _iperbooking;

        public RoomsProvider(IIperbooking iperbooking)
        {
            _iperbooking = iperbooking;
        }

        public IEnumerable<FlattenedRoom> Rooms { get; private set; } = new List<FlattenedRoom>();
        
        public async Task AccumulateAllRoomsAsync(string from, string to)
        {
            var (fromDate, toDate) = GetInitialRange(from, to);
            var rooms = await GetFlattenedRoomsAsync(fromDate, toDate);

            // extending search range to fetch all tiles that might possibly collide with
            // tiles that fall in the range, to ensure correct collision detection in front-end
            if (rooms.Any())
            {
                (fromDate, toDate) = GetExtendedRange(rooms);
                rooms = await GetFlattenedRoomsAsync(fromDate, toDate);
            }

            Rooms = rooms;
        }

        private (DateTime fromDate, DateTime toDate) GetInitialRange(string from, string to)
        {
            return
            (
                DateTime.ParseExact(from, "yyyy-MM-dd", null),
                DateTime.ParseExact(to, "yyyy-MM-dd", null)
            );
        }

        private (DateTime fromDate, DateTime toDate) GetExtendedRange(IEnumerable<FlattenedRoom> rooms)
        {
            var firstArrival = rooms.OrderBy(tile => tile.From).First();
            var lastDeparture = rooms.OrderBy(tile => tile.To).Last();

            return
            (
                firstArrival.From,
                lastDeparture.To.AddDays(-1)
            );
        }

        private async Task<IEnumerable<FlattenedRoom>> GetFlattenedRoomsAsync(DateTime fromDate, DateTime toDate)
        {
            var arrivalFrom = fromDate.AddDays(-30).ToString("yyyyMMdd");
            var arrivalTo = toDate.ToString("yyyyMMdd");

            var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);

            return bookings
                .SelectMany(
                    booking => booking.Rooms,
                    (booking, room) => new FlattenedRoom(
                        room.StayId.ToString(),
                        DateTime.ParseExact(room.Arrival, "yyyyMMdd", null),
                        DateTime.ParseExact(room.Departure, "yyyyMMdd", null),
                        booking,
                        room
                    ))
                .Where(roomData => (roomData.To - fromDate).Days >= 0);
        }
    }
}
