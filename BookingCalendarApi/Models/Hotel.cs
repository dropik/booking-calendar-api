namespace BookingCalendarApi.Models
{
    public class Hotel
    {
        public ICollection<Floor> Floors { get; set; } = new HashSet<Floor>();
    }
}
