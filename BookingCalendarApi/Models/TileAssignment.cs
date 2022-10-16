using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class TileAssignment
    {
        public TileAssignment(string id, string color)
        {
            Id = id;
            Color = color;
        }

        public string Id { get; private set; }
        [Required]
        public string Color { get; set; }
        public long? RoomId { get; set; }
    }
}
