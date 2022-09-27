namespace BookingCalendarApi.Models.Iperbooking
{
    public class Room
    {
        public string Roomname { get; set; } = "";
        public ICollection<Rategroup> Rategroups { get; set; } = new HashSet<Rategroup>();
    }
}
