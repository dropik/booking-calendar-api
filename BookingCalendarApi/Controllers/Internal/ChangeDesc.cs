namespace BookingCalendarApi.Controllers.Internal
{
    public class ChangeDesc
    {
        public ChangeDesc(string from, string to)
        {
            From = from;
            To = to;
        }

        public bool RoomChanged { get; set; } = false;
        public string From { get; set; }
        public string To { get; set; }
        public long? OriginalRoom { get; set; }
        public long? NewRoom { get; set; }
        public string? OriginalColor { get; set; }
        public string? NewColor { get; set; }
    }
}
