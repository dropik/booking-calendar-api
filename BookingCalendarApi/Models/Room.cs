using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Room
    {
        public Room(long id, long floorId, string number, string type)
        {
            Id = id;
            FloorId = floorId;
            Number = number;
            Type = type;
        }

        public long Id { get; private set; }
        [Required]
        public long FloorId { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public string Type { get; set; }

    }
}
