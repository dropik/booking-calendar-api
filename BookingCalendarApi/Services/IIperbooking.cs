namespace BookingCalendarApi.Services
{
    public interface IIperbooking
    {
        public Task<ICollection<Models.Iperbooking.RoomRates.Room>> GetRoomRatesAsync();
        public Task<ICollection<Models.Iperbooking.Bookings.Booking>> GetBookingsAsync(string arrivalFrom, string arrivalTo);
    }
}
