using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Iperbooking.RoomRates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        Task<List<RoomRateRoom>> GetRoomRates();
        Task<List<Booking>> GetBookings(string from, string to, bool exactPeriod = false);
        Task<GuestsResponse> GetGuests(string reservationId);
    }
}
