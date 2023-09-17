using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Extensions;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class CityTaxService : ICityTaxService
    {
        private readonly IAssignedBookingComposer _assignedBookingComposer;
        private readonly IIperbooking _iperbooking;
        private readonly DataContext _dataContext;
        private readonly IRepository _repository;

        private const int CHILD_AGE = 14;

        public CityTaxService(
            IAssignedBookingComposer assignedBookingComposer,
            IIperbooking iperbooking,
            DataContext dataContext,
            IRepository repository)
        {
            _assignedBookingComposer = assignedBookingComposer;
            _iperbooking = iperbooking;
            _dataContext = dataContext;
            _repository = repository;
        }

        public async Task<CityTaxResponse> Get(string from, string to)
        {
            _dataContext.RoomAssignments.AddRange(await _repository.RoomAssignments.ToListAsync());

            var bookings = (await _iperbooking.GetBookings(from, to))
                .ExcludeCancelled()
                .SelectInRange(from, to);

            var bookingIds = "";
            foreach (var booking in bookings)
            {
                bookingIds += $"{booking.BookingNumber},";
            }

            var guestResponse = await _iperbooking.GetGuests(bookingIds);

            var stays = bookings
                .UseComposer(_assignedBookingComposer)
                .SelectMany(
                    bookingContainer => bookingContainer.Rooms,
                    (bookingContainer, room) => new Stay(
                        stayId: room.StayId,
                        bookingNumber: bookingContainer.Booking.BookingNumber,
                        arrival: room.Arrival,
                        departure: room.Departure)
                    {
                        Guests = room.Guests,
                        RoomId = room.RoomId
                    })
                .ExcludeNotAssigned();

            var guests = guestResponse.Reservations
                .SelectMany(reservation => reservation.Guests, (reservation, guest) => new { guest.ReservationRoomId, guest.FirstName, guest.BirthDate })
                .Join(stays, guest => guest.ReservationRoomId, stay => stay.StayId, (guest, stay) => new { guest.FirstName, guest.BirthDate, stay.Arrival, stay.Departure });

            var result = new CityTaxResponse();

            foreach (var guest in guests)
            {
                if (guest.FirstName == "" || guest.BirthDate.Trim() == "")
                {
                    continue;
                }

                var guestResult = new CityTaxResponse();
                var nights = (DateTime.ParseExact(guest.Departure, "yyyyMMdd", null) - DateTime.ParseExact(guest.Arrival, "yyyyMMdd", null)).Days;
                var age = Utils.GetAgeAtArrival(guest.BirthDate, guest.Arrival);

                if (age < CHILD_AGE)
                {
                    guestResult.Children = nights;
                }
                else
                {
                    guestResult.Standard = Math.Min(nights, 10);
                    guestResult.Over10Days = Math.Max(nights - 10, 0);
                }

                var cropLeft = Math.Max((DateTime.ParseExact(from, "yyyy-MM-dd", null) - DateTime.ParseExact(guest.Arrival, "yyyyMMdd", null)).Days, 0);
                guestResult.Children = Math.Max(guestResult.Children - cropLeft, 0);
                guestResult.Standard -= cropLeft;
                if (guestResult.Standard < 0)
                {
                    guestResult.Over10Days = Math.Max(guestResult.Over10Days + guestResult.Standard, 0);
                    guestResult.Standard = 0;
                }

                var cropRight = Math.Max((DateTime.ParseExact(guest.Departure, "yyyyMMdd", null) - DateTime.ParseExact(to, "yyyy-MM-dd", null)).Days, 0);
                guestResult.Children = Math.Max(guestResult.Children - cropRight, 0);
                guestResult.Over10Days -= cropRight;
                if (guestResult.Over10Days < 0)
                {
                    guestResult.Standard = Math.Max(guestResult.Standard + guestResult.Over10Days, 0);
                    guestResult.Over10Days = 0;
                }

                result.Standard += guestResult.Standard;
                result.Children += guestResult.Children;
                result.Over10Days += guestResult.Over10Days;
            }

            return result;
        }
    }
}
