namespace BookingCalendarApi.Models.Iperbooking
{
    public class RateGroup
    {
        public ICollection<Rate> Rates { get; set; } = new HashSet<Rate>();
    }
}
