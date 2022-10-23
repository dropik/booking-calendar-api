using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingWithGuestsComposer : IComposer<AssignedBooking<Guest>, AssignedBooking<Models.Iperbooking.Guests.Guest>> { }
}
