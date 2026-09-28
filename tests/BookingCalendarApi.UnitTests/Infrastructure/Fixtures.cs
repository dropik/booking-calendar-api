using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

namespace BookingCalendarApi.UnitTests.Infrastructure
{
    /// <summary>Terse builders for Iperbooking data used across the tests.</summary>
    public static class Fixtures
    {
        public static Booking Booking(
            long number,
            params BookingRoom<BookingGuest>[] rooms) => Booking(number, "Mario", "Rossi", BookingStatus.New, rooms);

        public static Booking Booking(
            long number,
            string firstName,
            string lastName,
            BookingStatus status,
            params BookingRoom<BookingGuest>[] rooms)
        {
            return new Booking(number, firstName, lastName, "20230101120000")
            {
                Status = status,
                Rooms = rooms.ToList(),
            };
        }

        public static BookingRoom<BookingGuest> Room(long stayId, string arrival, string departure, int guests = 1, string roomName = "Doppia", string rateId = "R1")
        {
            return new BookingRoom<BookingGuest>(stayId, roomName, arrival, departure, rateId)
            {
                Guests = Enumerable.Range(0, guests).Select(_ => new BookingGuest(false)).ToList(),
            };
        }

        public static string TileId(long stayId, string arrival, string departure) => $"{stayId}-{arrival}-{departure}";

        public static Reservation Reservation(long reservationId, params Guest[] guests)
        {
            return new Reservation(reservationId) { Guests = guests.ToList() };
        }

        public static Guest Guest(
            string guestId,
            int stayId,
            string firstName = "Mario",
            string lastName = "Rossi",
            string birthDate = "19800101")
        {
            return new Guest(guestId, stayId, firstName, lastName, birthDate);
        }

        public static GuestsResponse Guests(params Reservation[] reservations) => new() { Reservations = reservations.ToList() };

        public static RoomRateRoom RoomRate(string roomName, params (string RateId, uint Min, uint Max)[] rates)
        {
            return new RoomRateRoom(roomName)
            {
                RateGroups = new List<RateGroup>
                {
                    new RateGroup
                    {
                        Rates = rates.Select(r => new Rate(r.RateId, r.Min, r.Max, "BB")).ToList(),
                    },
                },
            };
        }
    }
}
