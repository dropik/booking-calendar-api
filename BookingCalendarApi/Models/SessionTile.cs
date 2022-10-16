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

        public override bool Equals(object? obj)
        {
            return obj is SessionTile tile &&
                   TileId == tile.TileId &&
                   LastModified == tile.LastModified;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TileId, LastModified);
        }
    }
}
