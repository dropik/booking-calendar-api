using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class Booking
    {
        public Booking(long bookingNumber, string firstName, string lastName, string lastModified)
        {
            BookingNumber = bookingNumber;
            FirstName = firstName;
            LastName = lastName;
            LastModified = lastModified;
        }

        public long BookingNumber { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingStatus Status { get; set; } = BookingStatus.New;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BookingPaymentMethod PaymentMethod { get; set; } = BookingPaymentMethod.XX;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LastModified { get; set; }
        public decimal Deposit { get; set; }
        public bool DepositConfirmed { get; set; }

        public List<BookingRoom<BookingGuest>> Rooms { get; set; } = new List<BookingRoom<BookingGuest>>();
    }

    public enum BookingStatus {
        New,
        Modified,
        Cancelled,
    };

    public enum BookingPaymentMethod
    {
        XX,         // not available
        CC,         // Carta di credito
        BT,         // Bonifico bancario
        MO,         // Vaglia postale
        GC,         // Carta di credito a garanzia
        PT,         // Bollettino postale
        PP,         // PayPal
        CN,         // Carta di credito con Addebito
        CA,         // Contanti
    }
}