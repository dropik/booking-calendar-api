using BookingCalendarApi.Models.Iperbooking.RoomRates;

namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        public Task<ICollection<Room>> GetRoomRates();
    }
}
