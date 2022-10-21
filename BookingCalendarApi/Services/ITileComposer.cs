using BookingCalendarApi.Models;
using Room = BookingCalendarApi.Models.Iperbooking.Bookings.Room;

namespace BookingCalendarApi.Services
{
    public interface ITileComposer : IComposer<Room, Tile> { }
}
