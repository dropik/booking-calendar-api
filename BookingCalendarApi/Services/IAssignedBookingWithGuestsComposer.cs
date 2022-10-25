using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IAssignedBookingWithGuestsComposer : IComposer<AssignedBooking<Guest>, AssignedBooking<Models.Iperbooking.Guests.Guest>> { }
}
