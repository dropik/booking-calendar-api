using BookingCalendarApi.Models.Iperbooking.RoomRates;
using System.Collections.Generic;

namespace BookingCalendarApi.Models.Responses
{
    public class RoomRatesResponse
    {
        public List<RoomTypeResponse> RoomTypes { get; set; } = new List<RoomTypeResponse>();
        public List<Rate> RoomRates { get; set; } = new List<Rate>();
    }
}
