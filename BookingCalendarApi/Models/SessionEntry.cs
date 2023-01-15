namespace BookingCalendarApi.Models
{
    public class SessionEntry
    {
        public Guid Id { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
