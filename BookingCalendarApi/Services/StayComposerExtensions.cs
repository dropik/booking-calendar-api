using BookingCalendarApi.Models.Iperbooking.Bookings;

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
