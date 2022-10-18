using System.Text.Json.Serialization;

namespace BookingCalendarApi.Controllers.Internal
{
    public class LowerCaseEnumConverter : JsonStringEnumConverter
    {
        public LowerCaseEnumConverter() : base(System.Text.Json.JsonNamingPolicy.CamelCase, false) { }
    }
}
