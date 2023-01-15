using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        public Task<IEnumerable<RoomRateRoom>> GetRoomRates();
        public Task<List<Booking>> GetBookings(string from, string to, bool exactPeriod = false);
        public Task<GuestsResponse> GetGuests(string reservationId);
    }
}
