using BookingCalendarApi.Models.Iperbooking.Bookings;
using System.Collections.Generic;
using System.Linq;

namespace BookingCalendarApi.Services
{
    public static class StayComposerExtensions
    {
        public static IEnumerable<Stay> ExcludeNotAssigned(this IEnumerable<Stay> stays)
        {
            return stays.Where(stay => stay.RoomId != null);
        }
    }
}
