namespace BookingCalendarApi.Models.Iperbooking.Guests
{
    public class Reservation
    {
        public Reservation(long reservationId)
        {
            ReservationId = reservationId;
        }

        public long ReservationId { get; set; }
        public List<Guest> Guests { get; set; } = new();
    }
}
