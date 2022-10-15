using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class SessionItem
    {
        public SessionItem(Guid id, string tileId, string lastModified)
        {
            Id = id;
            TileId = tileId;
            LastModified = lastModified;
        }

        public Guid Id { get; set; }
        [Required]
        public string TileId { get; set; }
        [Required]
        public string LastModified { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is SessionItem session &&
                   Id.Equals(session.Id) &&
                   TileId == session.TileId &&
                   LastModified == session.LastModified;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, TileId, LastModified);
        }
    }
}
