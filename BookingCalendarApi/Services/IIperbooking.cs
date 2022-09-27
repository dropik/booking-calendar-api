using BookingCalendarApi.Models.Iperbooking;

namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        public Task<ICollection<Room>> GetRoomRates();
    }
}
