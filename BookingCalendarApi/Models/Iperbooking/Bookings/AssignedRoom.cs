﻿namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class AssignedRoom<TGuest> : Room<TGuest>
    {
        public AssignedRoom(long stayId, string roomName, string arrival, string departure) : base(stayId, roomName, arrival, departure) { }

        public long? RoomId { get; set; }
    }
}