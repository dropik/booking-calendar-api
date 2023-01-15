using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IBookingsService
    {
        Task<Booking<List<Client>>> Get(string id, string from);
        Task<List<ShortBooking>> GetByName(string from, string to, string? name);
        Task<BookingsBySessionResponse> GetBySession(string from, string to, string? sessionId);
        Task Ack(AckBookingsRequest request);
    }
}
