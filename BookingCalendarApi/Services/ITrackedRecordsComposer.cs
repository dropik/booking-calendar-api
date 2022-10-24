using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface ITrackedRecordsComposer : IComposer<AssignedBooking<Models.Iperbooking.Guests.Guest>, string> { }
}
