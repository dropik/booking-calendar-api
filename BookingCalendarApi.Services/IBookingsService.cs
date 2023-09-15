using BookingCalendarApi.Models.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IBookingsService
    {
        Task<List<BookingResponse<uint>>> GetByPeriod(string from, string to);
        Task<BookingResponse<List<ClientResponse>>> Get(string id, string from);
        Task<List<ShortBookingResponse>> GetByName(string from, string to, string name);
    }
}
