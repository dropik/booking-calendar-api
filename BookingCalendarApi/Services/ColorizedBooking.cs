using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class ColorizedBooking : Booking
    {
        public ColorizedBooking(long bookingNumber, string firstName, string lastName, string lastModified, string color) :
            base(bookingNumber, firstName, lastName, lastModified)
        {
            Color = color;
        }

        public string Color { get; set; }
    }
}
