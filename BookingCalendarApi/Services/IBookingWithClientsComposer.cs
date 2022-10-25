using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface IBookingWithClientsComposer : IComposer<Booking, Booking<IEnumerable<Client>>> { }
}
