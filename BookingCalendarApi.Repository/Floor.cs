using BookingCalendarApi.Repository.Common;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Repository
{
    public class Floor : IStructureData
    {
        public long Id { get; set; }
        public long StructureId { get; set; }
        [JsonIgnore]
        public Structure Structure { get; set; }
        public string Name { get; set; } = "";

        public List<Room> Rooms { get; set; } = new List<Room>();
    }
}
