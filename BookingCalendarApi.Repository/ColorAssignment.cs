using BookingCalendarApi.Repository.Common;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Repository
{
    public class ColorAssignment : IStructureData
    {
        public string BookingId { get; set; } = "";
     
        public long StructureId { get; set; }
        [JsonIgnore]
        public Structure Structure { get; set; }

        public string Color { get; set; } = "";
    }
}
