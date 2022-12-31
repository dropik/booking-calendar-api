using BookingCalendarApi.Models.Iperbooking.RoomRates;

namespace BookingCalendarApi.Models.Responses
{
    public class RoomRatesResponse
    {
        public List<RoomType> RoomTypes { get; set; } = new();
        public List<Rate> RoomRates { get; set; } = new();
    }
}
