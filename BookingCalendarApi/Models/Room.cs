using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Room
    {
        public string Id { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string Type { get; set; }

        public string FloorId { get; set; }
        public virtual Floor Floor { get; set; }
    }
}
