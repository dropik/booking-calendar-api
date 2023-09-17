using BookingCalendarApi.Repository.Common;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Repository
{
    public class Room : IStructureData
    {
        public long Id { get; set; }
        public long StructureId { get; set; }
        [JsonIgnore]
        public Structure Structure { get; set; }
        public long FloorId { get; set; }
        public string Number { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
