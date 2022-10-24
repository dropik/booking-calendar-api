using System.ComponentModel.DataAnnotations;

namespace BookingCalendarApi.Models
{
    public class PoliceNationCode
    {
        public PoliceNationCode(string iso, ulong code)
        {
            Iso = iso;
            Code = code;
        }

        [Key]
        public string Iso { get; set; }
        [Required]
        public ulong Code { get; set; }
    }
}
