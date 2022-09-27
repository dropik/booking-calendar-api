namespace BookingCalendarApi.Models.Iperbooking
{
    public class Rategroup
    {
        public ICollection<Rate> Rates { get; set; } = new HashSet<Rate>();
    }
}
