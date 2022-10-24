using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingWithGuestsProvider
    {
        public Task FetchAsync(string date);
        public IEnumerable<AssignedBooking<Models.Iperbooking.Guests.Guest>> Bookings { get; }
    }
}
