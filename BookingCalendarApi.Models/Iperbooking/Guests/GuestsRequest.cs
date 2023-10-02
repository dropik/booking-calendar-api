namespace BookingCalendarApi.Models.Iperbooking.Guests
{
    public class GuestsRequest
    {
        public GuestsRequest(int idhotel, string username, string password, string reservationId)
        {
            Idhotel = idhotel;
            Username = username;
            Password = password;
            ReservationId = reservationId;
        }

        public int Idhotel { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ReservationId { get; set; }
    }
}
