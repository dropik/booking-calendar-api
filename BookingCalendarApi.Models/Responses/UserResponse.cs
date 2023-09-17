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
        public List<RoomType> RoomTypes { get; set; } = new List<RoomType>();
        public List<Rate> RoomRates { get; set; } = new List<Rate>();
        public List<Floor> Floors { get; set; } = new List<Floor>();
    }

    public class RoomType
    {
        public string Name { get; set; }
        public uint MinOccupancy { get; set; }
        public uint MaxOccupancy { get; set; }

        public RoomType(string name, uint minOccupancy, uint maxOccupancy)
        {
            Name = name;
            MinOccupancy = minOccupancy;
            MaxOccupancy = maxOccupancy;
        }
    }
}
