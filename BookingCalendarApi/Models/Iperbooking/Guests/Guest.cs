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
        [JsonConverter(typeof(JsonEnumFromStringConverter<Sex>))]
        public Sex Gender { get; set; }
        public string BirthDate { get; set; }
        public string? BirthCountry { get; set; }
        public string? BirthCounty { get; set; }
        public string? BirthCity { get; set; }
        public string? Citizenship { get; set; }
        [JsonConverter(typeof(JsonEnumFromStringConverter<DocumentType>))]
        public DocumentType DocType { get; set; }
        public string? DocNumber { get; set; }
        public string? DocCountry { get; set; }
        public string? DocCity { get; set; }

        public enum Sex
        {
            M,
            F
        }

        public enum DocumentType
        {
            XX,
            ID,
            PP,
            DL
        }
    }
}
