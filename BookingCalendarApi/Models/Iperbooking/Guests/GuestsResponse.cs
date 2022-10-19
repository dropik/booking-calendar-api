namespace BookingCalendarApi.Models.Iperbooking.Guests
{
    public class GuestsResponse
    {
        public IEnumerable<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
