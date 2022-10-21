using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Internal
{
    internal class LowerCaseEnumConverter : JsonStringEnumConverter
    {
        public LowerCaseEnumConverter() : base(System.Text.Json.JsonNamingPolicy.CamelCase, false) { }
    }
}
