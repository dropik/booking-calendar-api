using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;

namespace BookingCalendarApi.Services
{
    public interface IAssignedBookingWithGuestsProvider
    {
        public Task<List<AssignedBooking<Guest>>> Get(string from, string? to = null, bool exactPeriod = true);
    }
}
