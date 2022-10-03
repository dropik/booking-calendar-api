namespace BookingCalendarApi.Models
{
    public class TileResponse
    {
        public IEnumerable<Tile> Tiles { get; set; }
        public string SessionId { get; set; }
    }
}
