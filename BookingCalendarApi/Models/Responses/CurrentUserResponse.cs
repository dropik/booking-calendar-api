using BookingCalendarApi.Models.Entities;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

namespace BookingCalendarApi.Models.Responses
{
    public class CurrentUserResponse
    {
        public string Username { get; set; } = "";
        public string VisibleName { get; set; } = "";
        public List<RoomTypeResponse> RoomTypes { get; set; } = new();
        public List<Rate> RoomRates { get; set; } = new();
        public List<Floor> Floors { get; set; } = new();
    }
}
