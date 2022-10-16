namespace BookingCalendarApi.Models
{
    public class SessionTile
    {
        public SessionTile(string tileId, string lastModified)
        {
            TileId = tileId;
            LastModified = lastModified;
        }

        public string TileId { get; set; }
        public string LastModified { get; set; }
    }
}
