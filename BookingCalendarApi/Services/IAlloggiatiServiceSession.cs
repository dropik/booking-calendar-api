using AlloggiatiService;
using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public interface IAlloggiatiServiceSession
    {
        Task OpenAsync();
        Task SendDataAsync(IList<string> data, bool test);
        Task<byte[]> GetRicevutaAsync(DateTime date);
        Task<List<Place>> GetPlacesAsync();
    }
}
