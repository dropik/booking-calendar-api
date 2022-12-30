using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        public Task<IEnumerable<RoomRateRoom>> GetRoomRatesAsync();
        public Task<IEnumerable<Booking>> GetBookingsAsync(string arrivalFrom, string arrivalTo);
        public Task<GuestsResponse> GetGuestsAsync(string reservationId);
    }
}
