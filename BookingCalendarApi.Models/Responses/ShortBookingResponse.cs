using BookingCalendarApi.Models.Json;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Responses
{
    public class ShortBookingResponse
    {
        public string Id { get; set; }
        [JsonConverter(typeof(LowerCaseEnumConverter))]
        public BookingStatus Status { get; set; } = BookingStatus.New;
        public string Name { get; set; }
        public string LastModified { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Color { get; set; } = null;
        public uint Occupations { get; set; }

        public ShortBookingResponse(string id, string name, string lastModified, string from, string to, uint occupations)
        {
            Id = id;
            Name = name;
            LastModified = lastModified;
            From = from;
            To = to;
            Occupations = occupations;
        }
    }
}
