using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface ITileWithClientsComposer : IComposer<Room<Guest>, Tile<IEnumerable<Client>>> { }
}
