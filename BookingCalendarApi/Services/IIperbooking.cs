namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        public Task<IEnumerable<Models.Iperbooking.RoomRates.Room>> GetRoomRatesAsync();
        public Task<IEnumerable<Models.Iperbooking.Bookings.Booking>> GetBookingsAsync(string arrivalFrom, string arrivalTo);
    }
}
