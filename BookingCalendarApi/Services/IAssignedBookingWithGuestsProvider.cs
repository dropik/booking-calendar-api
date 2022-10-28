using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IAssignedBookingWithGuestsProvider
    {
        public Task FetchAsync(string from, string? to = null, bool exactPeriod = true);
        public IEnumerable<AssignedBooking<Models.Iperbooking.Guests.Guest>> Bookings { get; }
    }
}
