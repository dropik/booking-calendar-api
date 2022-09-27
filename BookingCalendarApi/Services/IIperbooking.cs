using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        public ICollection<Room> GetRoomRates();
    }
}
