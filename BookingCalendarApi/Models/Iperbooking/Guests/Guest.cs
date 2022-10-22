using BookingCalendarApi.Models.Internal;
using System.Text.Json.Serialization;

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

        [JsonConverter(typeof(GuestIdConverter))]
        public string GuestId { get; set; }
        public int ReservationRoomId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BirthDate { get; set; }
        public string? BirthCountry { get; set; }
        public string? BirthCity { get; set; }
    }
}
