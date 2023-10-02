using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Configurations
{
    public class ApiSettings
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Environment Environment { get; set; }
    }

    public enum Environment
    {
        Development,
        Production,
    }
}
