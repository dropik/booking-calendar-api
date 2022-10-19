namespace BookingCalendarApi.Models.Iperbooking.Guests
{
    public class Reservation
    {
        public Reservation(int id)
        {
            ReservationId = id;
        }

        public int ReservationId { get; set; }
        public IEnumerable<Guest> Guests { get; set; } = new List<Guest>();
    }
}
