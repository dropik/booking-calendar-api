using BookingCalendarApi.Models.Responses;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface ICityTaxService
    {
        Task<CityTaxResponse> Get(string from, string to);
    }
}
