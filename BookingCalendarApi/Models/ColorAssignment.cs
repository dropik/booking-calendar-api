using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class ColorAssignment
    {
        public ColorAssignment(string bookingId, string color)
        {
            BookingId = bookingId;
            Color = color;
        }

        [Key]
        public string BookingId { get; set; }
        [Required]
        public string Color { get; set; }
    }
}
