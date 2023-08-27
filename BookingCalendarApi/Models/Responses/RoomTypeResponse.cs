﻿namespace BookingCalendarApi.Models.Responses
{
    public class RoomTypeResponse
    {
        public RoomTypeResponse(string name, uint minOccupancy, uint maxOccupancy)
        {
            Name = name;
            MinOccupancy = minOccupancy;
            MaxOccupancy = maxOccupancy;
        }

        public string Name { get; set; }
        public uint MinOccupancy { get; set; }
        public uint MaxOccupancy { get; set; }
    }
}