namespace BookingCalendarApi.Models.Requests
{
    public class AssignmentsRequest
    {
        public Dictionary<string, string> Colors { get; set; } = new();
        public Dictionary<string, long?> Rooms { get; set; } = new();
    }
}
