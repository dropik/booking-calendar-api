﻿namespace BookingCalendarApi.Models.Iperbooking.RoomRates
{
    public class Room
    {
        public string RoomName { get; set; } = "";
        public ICollection<RateGroup> RateGroups { get; set; } = new HashSet<RateGroup>();
    }
}