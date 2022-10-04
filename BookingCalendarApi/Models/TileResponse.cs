namespace BookingCalendarApi.Models
{
    public class TileResponse
    {
        public TileResponse(string sessionId)
        {
            SessionId = sessionId;
        }

        public IEnumerable<Tile> Tiles { get; set; } = new List<Tile>();
        public string SessionId { get; set; }
    }
}
