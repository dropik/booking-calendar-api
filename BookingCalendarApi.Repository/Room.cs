namespace BookingCalendarApi.Repository
{
    public class Room
    {
        public long Id { get; set; }
        public long FloorId { get; set; }
        public string Number { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
