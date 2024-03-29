﻿using System.Collections.Generic;

namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class Stay
    {
        public Stay(long stayId, long bookingNumber, string arrival, string departure)
        {
            StayId = stayId;
            BookingNumber = bookingNumber;
            Arrival = arrival;
            Departure = departure;
        }

        public long StayId { get; set; }
        public long BookingNumber { get; set; }
        public string Arrival { get; set; }
        public string Departure { get; set; }
        public long? RoomId { get; set; }

        public List<BookingGuest> Guests { get; set; } = new List<BookingGuest>();
    }
}
