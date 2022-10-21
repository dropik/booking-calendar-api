using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public class CityTaxComposer : ICityTaxComposer
    {
        public IEnumerable<CityTax> Compose(IEnumerable<Models.Iperbooking.Bookings.Booking> source)
        {
            return source
                .SelectMany(
                    booking => booking.Rooms,
                    (booking, room) => new CityTax()
                    {
                        Standard = Convert.ToUInt32(
                            room.Guests.Count *
                            (DateTime.ParseExact(room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(room.Arrival, "yyyyMMdd", null)).Days
                        )
                    }
                );
        }
    }
}
