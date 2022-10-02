using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Room
    {
        public Room(long id, long floorId)
        {
            Id = id;
            FloorId = floorId;
        }

        public long Id { get; private set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string Type { get; set; }

        public long FloorId { get; set; }
    }
}
