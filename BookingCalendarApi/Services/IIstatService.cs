using BookingCalendarApi.Models.DTO;
namespace BookingCalendarApi.Services
{
    public interface IIstatService
    {
        Task<IstatMovementsDTO> GetMovements();
        Task SendMovements(IstatMovementsDTO movements);
        Task<List<string>> GetCountries();
    }
}
