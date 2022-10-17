namespace BookingCalendarApi.Models
{
    public class RoomAssignment
    {
        public RoomAssignment(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }
        public long? RoomId { get; set; }
    }
}
