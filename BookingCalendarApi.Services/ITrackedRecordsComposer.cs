using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;

namespace BookingCalendarApi.Services
{
    public interface ITrackedRecordsComposer : IComposer<AssignedBooking<Guest>, string> { }
}
