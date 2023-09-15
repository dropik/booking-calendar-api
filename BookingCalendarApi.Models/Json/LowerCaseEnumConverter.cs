using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Json
{
    public class LowerCaseEnumConverter : JsonStringEnumConverter
    {
        public LowerCaseEnumConverter() : base(System.Text.Json.JsonNamingPolicy.CamelCase, false) { }
    }
}
