namespace BookingCalendarApi.Models
{
    public class SessionEntry
    {
        public SessionEntry(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }
        public byte[] Data { get; set; } = Array.Empty<byte>();
    }
}
