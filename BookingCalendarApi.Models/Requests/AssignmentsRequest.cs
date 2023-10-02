using System.Collections.Generic;

namespace BookingCalendarApi.Models.Requests
{
    public class AssignmentsRequest
    {
        public Dictionary<string, string> Colors { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, long?> Rooms { get; set; } = new Dictionary<string, long?>();
    }
}
