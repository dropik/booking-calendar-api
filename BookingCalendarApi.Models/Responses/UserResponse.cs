using BookingCalendarApi.Models.Iperbooking.RoomRates;
using BookingCalendarApi.Repository;
using System.Collections.Generic;

namespace BookingCalendarApi.Models.Responses
{
    public class UserResponse
    {
        public string Username { get; set; } = "";
        public string VisibleName { get; set; } = "";
        public string Structure { get; set; } = "";
        public List<RoomTypeResponse> RoomTypes { get; set; } = new List<RoomTypeResponse>();
        public List<Rate> RoomRates { get; set; } = new List<Rate>();
        public List<Floor> Floors { get; set; } = new List<Floor>();
    }
}
