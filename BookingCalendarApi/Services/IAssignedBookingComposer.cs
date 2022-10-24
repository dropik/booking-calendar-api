using BookingCalendarApi.Models.Iperbooking.Bookings;
using Booking = BookingCalendarApi.Models.Iperbooking.Bookings.Booking;

namespace BookingCalendarApi.Services
{
    public interface IAssignedBookingComposer : IComposer<Booking, AssignedBooking<Guest>> { }
}
