using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public interface IAlloggiatiTableReader
    {
        public List<Place> ReadAsPlaces(string data);
    }
}
