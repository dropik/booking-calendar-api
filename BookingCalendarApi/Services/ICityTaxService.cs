using BookingCalendarApi.Models.Responses;

namespace BookingCalendarApi.Services
{
    public interface ICityTaxService
    {
        Task<CityTaxResponse> Get(string from, string to);
    }
}
