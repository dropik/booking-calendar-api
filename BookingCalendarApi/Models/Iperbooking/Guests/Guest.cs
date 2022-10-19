namespace BookingCalendarApi.Models.Iperbooking.Guests
{
    public class Guest
    {
        public Guest(string guestId, int reservationRoomId, string firstName, string lastName, string birthDate)
        {
            GuestId = guestId;
            ReservationRoomId = reservationRoomId;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public string GuestId { get; set; }
        public int ReservationRoomId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string? BirthCountry { get; set; }
        public string? BirthCity { get; set; }
    }
}
