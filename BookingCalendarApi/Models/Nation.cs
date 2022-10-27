using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class Nation
    {
        public Nation(string iso, ulong code, string description)
        {
            Iso = iso;
            Code = code;
            Description = description;
        }

        [Key]
        public string Iso { get; set; }
        [Required]
        public ulong Code { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
