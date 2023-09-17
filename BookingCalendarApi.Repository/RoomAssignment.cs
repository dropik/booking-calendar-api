using BookingCalendarApi.Repository.Common;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Repository
{
    public class RoomAssignment : IStructureData
    {
        public string Id { get; set; } = "";
        public long StructureId { get; set; }
        [JsonIgnore]
        public Structure Structure { get; set; }
        public long RoomId { get; set; }
        [JsonIgnore]
        public Room Room { get; set; }
    }
}
