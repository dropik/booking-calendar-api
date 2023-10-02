using BookingCalendarApi.Models.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IIstatService
    {
        Task<IstatMovementsDTO> GetMovements();
        Task SendMovements(IstatMovementsDTO movements);
        Task<List<string>> GetCountries();
    }
}
