using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Floor
    {
        public Floor(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public long Id { get; private set; }
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
