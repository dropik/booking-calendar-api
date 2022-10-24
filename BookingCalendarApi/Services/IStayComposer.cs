using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IStayComposer : IComposer<AssignedBooking<Guest>, Stay> { }
}
