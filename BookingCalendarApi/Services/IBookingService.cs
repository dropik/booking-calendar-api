using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IBookingService
    {
        Task<Booking<List<Client>>> Get(string id, string from);
    }
}
