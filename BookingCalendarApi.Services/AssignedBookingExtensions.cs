using BookingCalendarApi.Models.Iperbooking.Bookings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingCalendarApi.Services
{
    public static class AssignedBookingExtensions
    {
        public static IEnumerable<AssignedBooking<BookingGuest>> ExcludeNotAssigned(this IEnumerable<AssignedBooking<BookingGuest>> bookings) =>
            bookings
            .Select(booking => new AssignedBooking<BookingGuest>(booking.Booking)
            {
                Rooms = booking.Rooms
                    .Where(room => room.RoomId != null)
                    .ToList()
            })
            .Where(booking => booking.Rooms.Any());

        public static IEnumerable<AssignedBooking<TGuest>> SelectByArrival<TGuest>(this IEnumerable<AssignedBooking<TGuest>> bookings, DateTime arrival) =>
            bookings
            .Select(booking => new AssignedBooking<TGuest>(booking.Booking)
            {
                Rooms = booking.Rooms
                    .Where(room => (DateTime.ParseExact(room.Arrival, "yyyyMMdd", null) - arrival).Days == 0)
                    .ToList()
            })
            .Where(booking => booking.Rooms.Any());
    }
}
