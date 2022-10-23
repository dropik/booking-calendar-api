﻿namespace BookingCalendarApi.Models.Iperbooking.Bookings
{
    public class AssignedBooking
    {
        public AssignedBooking(Booking booking)
        {
            Booking = booking;
        }

        public Booking Booking { get; set; }
        public IEnumerable<AssignedRoom<Guest>> Rooms { get; set; } = new List<AssignedRoom<Guest>>();
    }
}
