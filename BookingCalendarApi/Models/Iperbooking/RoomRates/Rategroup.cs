namespace BookingCalendarApi.Models.Iperbooking.RoomRates
{
    public class RateGroup
    {
        public ICollection<Rate> Rates { get; set; } = new List<Rate>();
    }
}
