using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.Services
{
    public interface IBookingsService
    {
        Task<BookingResponse<List<ClientResponse>>> Get(string id, string from);
        Task<List<ShortBookingResponse>> GetByName(string from, string to, string? name);
        Task<BookingsBySessionResponse> GetBySession(string from, string to, string? sessionId);
        Task Ack(AckBookingsRequest request);
    }
}
