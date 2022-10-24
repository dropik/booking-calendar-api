namespace BookingCalendarApi.Models.Iperbooking.Guests
{
    public class Reservation
    {
        public Reservation(long reservationId)
        {
            ReservationId = reservationId;
        }

        public long ReservationId { get; set; }
        public IEnumerable<Guest> Guests { get; set; } = new List<Guest>();
    }
}
