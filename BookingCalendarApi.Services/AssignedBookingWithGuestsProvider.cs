using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class AssignedBookingWithGuestsProvider : IAssignedBookingWithGuestsProvider
    {
        private readonly IRepository _repository;
        private readonly IAssignedBookingComposer _assignedBookingComposer;
        private readonly IIperbooking _iperbooking;
        private readonly DataContext _dataContext;

        public AssignedBookingWithGuestsProvider(
            IRepository repository,
            IAssignedBookingComposer assignedBookingComposer,
            IIperbooking iperbooking,
            DataContext dataContext)
        {
            _repository = repository;
            _assignedBookingComposer = assignedBookingComposer;
            _iperbooking = iperbooking;
            _dataContext = dataContext;
        }

        public async Task<List<AssignedBooking<Guest>>> Get(string from, string to = null, bool exactPeriod = true)
        {
            if (to == null)
            {
                to = DateTime.ParseExact(from, "yyyy-MM-dd", null).AddDays(1).ToString("yyyy-MM-dd");
            }

            var bookings = (await _iperbooking.GetBookings(from, to, exactPeriod))
                .ExcludeCancelled();

            var stayIds = bookings
                .SelectMany(
                    booking => booking.Rooms,
                    (booking, room) => $"{room.StayId}-{room.Arrival}-{room.Departure}"
            );

            _dataContext.RoomAssignments.AddRange(await _repository.RoomAssignments.Where(assignment => stayIds.Contains(assignment.Id)).ToListAsync());

            var assignedBookings = bookings
                .UseComposer(_assignedBookingComposer)
                .ExcludeNotAssigned();

            var bookingIds = "";
            foreach (var booking in assignedBookings)
            {
                bookingIds += $"{booking.Booking.BookingNumber},";
            }

            var guestResponse = await _iperbooking.GetGuests(bookingIds);

            return assignedBookings
                .Join(
                    guestResponse.Reservations,
                    booking => booking.Booking.BookingNumber,
                    reservation => reservation.ReservationId,
                    (booking, reservation) => new AssignedBooking<Guest>(booking.Booking)
                    {
                        Rooms = booking.Rooms
                            .Select(
                                room => new AssignedRoom<Guest>(
                                stayId: room.StayId,
                                roomName: room.RoomName,
                                arrival: room.Arrival,
                                departure: room.Departure,
                                rateId: room.RateId)
                                {
                                    RoomId = room.RoomId,
                                    Guests = reservation.Guests
                                        .Where(guest => guest.ReservationRoomId == room.StayId)
                                        .Where(guest => guest.FirstName != "")
                                        .ToList()
                                })
                            .Where(room => room.Guests.Any())
                            .ToList()
                    }
                )
                .Where(booking => booking.Rooms.Any())
                .ToList();
        }
    }
}
