using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Json
{
    internal class LowerCaseEnumConverter : JsonStringEnumConverter
    {
        public LowerCaseEnumConverter() : base(System.Text.Json.JsonNamingPolicy.CamelCase, false) { }
    }
}
