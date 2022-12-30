using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface ITileWithClientsComposer : IComposer<BookingRoom<BookingGuest>, Tile<IEnumerable<Client>>> { }
}
