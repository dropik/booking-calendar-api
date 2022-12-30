using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public interface ITileComposer : IComposer<BookingRoom<BookingGuest>, Tile<uint>> { }
}
