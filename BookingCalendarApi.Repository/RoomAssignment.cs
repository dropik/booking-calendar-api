using System.Text.Json.Serialization;

namespace BookingCalendarApi.Repository
{
    public class RoomAssignment
    {
        public string Id { get; set; } = "";
        public long RoomId { get; set; }
        [JsonIgnore]
        public Room Room { get; set; }
    }
}
