namespace BookingCalendarApi.Models.Internal
{
    internal class AssignedTile
    {
        public AssignedTile(DateTime arrival, DateTime departure, long roomId)
        {
            Arrival = arrival;
            Departure = departure;
            RoomId = roomId;
        }

        public DateTime Arrival { get; set; }
        public DateTime Departure { get; set; }
        public long RoomId { get; set; }
    }
}
