using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;

namespace BookingCalendarApi.IntegrationTests.Infrastructure
{
    public static class IperbookingData
    {
        public static Booking Booking(long number, params BookingRoom<BookingGuest>[] rooms) => Booking(number, "Mario", "Rossi", rooms);

        public static Booking Booking(long number, string firstName, string lastName, params BookingRoom<BookingGuest>[] rooms)
            => new(number, firstName, lastName, "20230501101010") { Rooms = rooms.ToList() };

        public static BookingRoom<BookingGuest> Room(long stayId, string arrival, string departure, int guests = 1, string roomName = "Doppia")
            => new(stayId, roomName, arrival, departure, "R1") { Guests = Enumerable.Range(0, guests).Select(_ => new BookingGuest(false)).ToList() };

        public static Reservation Reservation(long id, params Guest[] guests) => new(id) { Guests = guests.ToList() };

        public static Guest Guest(string id, int stayId, string firstName, string lastName, string birthDate, Action<Guest>? configure = null)
        {
            var guest = new Guest(id, stayId, firstName, lastName, birthDate);
            configure?.Invoke(guest);
            return guest;
        }
    }

    public record BookingDto(string Id, string Status, string Name, string From, string To, string? Color, List<TileDto> Tiles);

    public record TileDto(string Id, string From, uint Nights, string RoomType, long? RoomId);
}
