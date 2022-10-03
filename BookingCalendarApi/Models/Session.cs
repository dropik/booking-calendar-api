using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Session
    {
        public Guid Id { get; set; }
        public string TileId { get; set; }
    }
}
