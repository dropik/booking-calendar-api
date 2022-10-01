using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Floor
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
