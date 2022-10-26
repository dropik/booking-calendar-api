using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;

namespace BookingCalendarApi.Services
{
    public class CityTaxGuestRegistriesFilter : CityTaxCalculatorDecoratorBase
    {
        private readonly IEnumerable<Reservation> _reservations;

        public CityTaxGuestRegistriesFilter(IEnumerable<Reservation> reservations, ICityTaxCalculator wrappee) : base(wrappee)
        {
            _reservations = reservations;
        }

        public override CityTax Calculate(Stay stay)
        {
            var value = base.Calculate(stay);

            var reservation = _reservations.SingleOrDefault(reservation => reservation.ReservationId == stay.BookingNumber);
            if (reservation == null)
            {
                return new();
            }

            var guestsInStay = reservation.Guests
                .Where(guest => guest.ReservationRoomId == stay.StayId)
                .ToList();
            var stayGuestsCount = stay.Guests.Count();
            var nights = value.Standard / stayGuestsCount;
            if (stayGuestsCount > guestsInStay.Count)
            {
                var diff = stayGuestsCount - guestsInStay.Count;
                value.Standard -= (uint)(diff * nights);
            }

            foreach (var guest in guestsInStay)
            {
                if (guest.FirstName == "" || guest.BirthDate.Trim() == "")
                {
                    value.Standard -= (uint)nights;
                }
                else if (GetAgeAtArrival(guest.BirthDate, stay.Arrival) < 14)
                {
                    value.Standard -= (uint)nights;
                    value.Children += (uint)nights;
                }
            }

            return value;
        }

        public static int GetAgeAtArrival(string birthDate, string arrival)
        {
            var birthDateObj = DateTime.ParseExact(birthDate, "yyyyMMdd", null);
            var arrivalObj = DateTime.ParseExact(arrival, "yyyyMMdd", null);

            int years = arrivalObj.Year - birthDateObj.Year;

            if (
                (
                    birthDateObj.Month == arrivalObj.Month &&
                    arrivalObj.Day < birthDateObj.Day
                ) ||
                arrivalObj.Month < birthDateObj.Month
            )
            {
                years--;
            }

            return years;
        }
    }
}
