using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IClientsService
    {
        Task<List<ClientWithBooking>> GetByQuery(string query, string from, string to);
    }
}
