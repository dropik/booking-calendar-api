using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        public Task<IEnumerable<RoomRateRoom>> GetRoomRatesAsync();
        public Task<List<Booking>> GetBookingsAsync(string from, string to, bool exactPeriod = false);
        public Task<GuestsResponse> GetGuestsAsync(string reservationId);
    }
}
