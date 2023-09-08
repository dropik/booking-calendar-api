using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class ColorizedBooking : Booking
    {
        public ColorizedBooking(Booking booking, string color) :
            base(booking.BookingNumber, booking.FirstName, booking.LastName, booking.LastModified)
        {
            Status = booking.Status;
            Rooms = booking.Rooms;
            Color = color;
        }

        public string Color { get; set; } = null;
    }
}
