using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookingCalendarApi.Services
{
    public static class ClientExtensions
    {
        public static IEnumerable<Reservation> SelectByStayId(this IEnumerable<Reservation> reservations, string stayId)
        {
            return reservations
                .Select(reservation => new Reservation(reservation.ReservationId)
                {
                    Guests = reservation.Guests
                        .Where(guest => guest.ReservationRoomId.ToString() == stayId)
                        .ToList()
                });
        }

        public static IEnumerable<Reservation> SelectByQuery(this IEnumerable<Reservation> reservations, string query)
        {
            return reservations
                .Select(reservation => new Reservation(reservation.ReservationId)
                {
                    Guests = reservation.Guests
                        .Where(guest =>
                            guest.FirstName != string.Empty
                            && $"{guest.FirstName} {guest.LastName}".ToLower().Contains(query.ToLower()))
                        .ToList()
                });
        }

        public static IEnumerable<ClientResponse> ComposeResponse(this IEnumerable<Reservation> reservations)
        {
            return reservations
                .SelectMany(
                    x => x.Guests,
                    (reservation, guest) => new ClientResponse(
                        id:             guest.GuestId,
                        bookingId:      reservation.ReservationId.ToString(),
                        name:           guest.FirstName,
                        surname:        guest.LastName,
                        dateOfBirth:    guest.BirthDate.Trim() != "" ? DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null).ToString("yyyy-MM-dd") : ""
                    )
                    {
                        StateOfBirth = guest.BirthCountry,
                        PlaceOfBirth = guest.BirthCity,
                        ProvinceOfBirth = guest.BirthCounty
                    }
                );
        }
    }
}
