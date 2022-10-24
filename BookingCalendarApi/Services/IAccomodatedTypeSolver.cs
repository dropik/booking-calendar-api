using BookingCalendarApi.Models.AlloggiatiService;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IAccomodatedTypeSolver
    {
        public void Solve(TrackedRecord record, IEnumerable<TrackedRecord> recordsBlock, AssignedBooking<Models.Iperbooking.Guests.Guest> booking);
    }
}
