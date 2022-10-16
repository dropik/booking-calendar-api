namespace BookingCalendarApi.Controllers
{
    public class ChangeDesc
    {
        public ChangeDesc(string from)
        {
            From = from;
        }

        public bool RoomChanged { get; set; } = false;
        public string From { get; set; }
        public ulong? OriginalRoom { get; set; }
        public ulong? NewRoom { get; set; }
        public string? OriginalColor { get; set; }
        public string? NewColor { get; set; }
    }
}
