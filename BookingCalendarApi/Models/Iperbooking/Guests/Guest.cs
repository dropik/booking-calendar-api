namespace BookingCalendarApi.Models.Iperbooking.Guests
{
    public class Guest
    {
        public Guest(int id, int reservationRoomId, string firstName, string lastName, string birthDate)
        {
            GuestId = id;
            ReservationRoomId = reservationRoomId;
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
        }

        public int GuestId { get; set; }
        public int ReservationRoomId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string? BirthCountry { get; set; }
        public string? BirthCity { get; set; }
    }
}
