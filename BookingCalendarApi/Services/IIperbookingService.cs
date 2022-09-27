using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IIperbookingService
    {
        public ICollection<Room> GetRoomRates();
    }
}
