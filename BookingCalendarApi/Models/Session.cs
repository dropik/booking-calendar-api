using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Session
    {
        public Session(Guid id, string tileId, string lastModified)
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
    }
}
