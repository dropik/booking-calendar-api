using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Session
    {
        public Session(Guid id, string tileId)
        {
            Id = id;
            TileId = tileId;
        }
        
        public Guid Id { get; set; }
        [Required]
        public string TileId { get; set; }
    }
}
