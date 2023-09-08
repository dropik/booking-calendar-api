using BookingCalendarApi.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IClientsService
    {
        Task<List<ClientWithBookingResponse>> GetByQuery(string query, string from, string to);
        Task<List<ClientResponse>> GetByTile(string bookingId, string tileId);
    }
}
