﻿using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class BookingsService : IBookingsService
    {
        private readonly IIperbooking _iperbooking;
        private readonly IRepository _repository;

        public BookingsService(IIperbooking iperbooking, IRepository repository)
        {
            _iperbooking = iperbooking;
            _repository = repository;
        }

        public async Task<BookingResponse<List<ClientResponse>>> Get(string id, string from)
        {
            var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
            var arrivalFrom = fromDate.AddDays(-15).ToString("yyyy-MM-dd");
            var arrivalTo = fromDate.AddDays(15).ToString("yyyy-MM-dd");
            var bookings = await _iperbooking.GetBookings(arrivalFrom, arrivalTo, exactPeriod: true);
            var guests = await _iperbooking.GetGuests(id);

            var booking = bookings.SelectById(id);

            var query = (from b in booking
                         join color in _repository.ColorAssignments on b.BookingNumber.ToString() equals color.BookingId into gj
                         from color in gj.DefaultIfEmpty()
                         select new { Booking = b, Color = color }
                        ).ToList()
                        .Select(join => new BookingResponse<List<ClientResponse>>(
                            id: join.Booking.BookingNumber.ToString(),
                            name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                            lastModified: join.Booking.LastModified,
                            from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                            to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                            deposit: join.Booking.Deposit,
                            depositConfirmed: join.Booking.DepositConfirmed,
                            isBankTransfer: join.Booking.PaymentMethod == BookingPaymentMethod.BT)
                        {
                            Status = join.Booking.Status,
                            Color = join.Color?.Color,
                            Tiles = (from room in @join.Booking.Rooms
                            join reservation in guests.Reservations on @join.Booking.BookingNumber equals reservation.ReservationId
                                     join assignment in _repository.RoomAssignments on $"{room.StayId}-{room.Arrival}-{room.Departure}" equals assignment.Id into gj
                                     from assignment in gj.DefaultIfEmpty()
                                     select new { Room = room, Reservation = reservation, Assignment = assignment })
                                    .ToList()
                                    .Select(x => new TileResponse<List<ClientResponse>>(
                                        id: $"{x.Room.StayId}-{x.Room.Arrival}-{x.Room.Departure}",
                                        from: DateTime.ParseExact(x.Room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                        nights: Convert.ToUInt32((DateTime.ParseExact(x.Room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(x.Room.Arrival, "yyyyMMdd", null)).Days),
                                        roomType: x.Room.RoomName,
                                        rateId: x.Room.RateId,
                                        persons: x.Reservation.Guests
                                            .Where(guest => guest.ReservationRoomId == x.Room.StayId)
                                            .Select(guest => new ClientResponse(
                                                id: guest.GuestId,
                                                bookingId: x.Reservation.ReservationId.ToString(),
                                                name: guest.FirstName,
                                                surname: guest.LastName,
                                                dateOfBirth: guest.BirthDate.Trim() != string.Empty ? DateTime.ParseExact(guest.BirthDate, "yyyyMMdd", null).ToString("yyyy-MM-dd") : ""
                                            )
                                            {
                                                PlaceOfBirth = guest.BirthCity,
                                                ProvinceOfBirth = guest.BirthCounty,
                                                StateOfBirth = guest.BirthCountry
                                            })
                                            .ToList())
                                    {
                                        RoomId = x.Assignment?.RoomId
                                    })
                                    .ToList()
                        });

            if (!query.Any())
            {
                throw new BookingCalendarException(BCError.NOT_FOUND, "No booking found.");
            }

            return query.First();
        }

        public async Task<List<ShortBookingResponse>> GetByName(string from, string to, string name)
        {
            var definedName = name ?? "";
            var bookingsByName = (await _iperbooking.GetBookings(from, to))
                .SelectInRange(from, to)
                .SelectByName(definedName);

            var bookingsWithColors = from booking in bookingsByName
                                     join color in _repository.ColorAssignments on booking.BookingNumber.ToString() equals color.BookingId into gj
                                     from color in gj.DefaultIfEmpty()
                                     select new { Booking = booking, Color = color };

            return bookingsWithColors
                .Select(join => new ShortBookingResponse(
                    id: join.Booking.BookingNumber.ToString(),
                    name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                    lastModified: join.Booking.LastModified,
                    from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                    to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                    occupations: Convert.ToUInt32(join.Booking.Rooms.Count)
                )
                {
                    Status = join.Booking.Status,
                    Color = join.Color?.Color
                })
                .ToList();
        }

        public async Task<List<BookingResponse<uint>>> GetByPeriod(string from, string to)
        {
            var loadedBookings = await _iperbooking.GetBookings(from, to);
            var bookings = loadedBookings.SelectInRange(from, to, true);

            var bookingNumbers = bookings.Select(b => b.BookingNumber.ToString()).ToList();
            var colorAssignments = await (from ca in _repository.ColorAssignments
                                          where bookingNumbers.Contains(ca.BookingId)
                                          select ca).ToListAsync();

            var roomIds = bookings
                .SelectMany(b => b.Rooms, (b, r) => $"{r.StayId}-{r.Arrival}-{r.Departure}")
                .ToList();
            var roomAssignments = await (from ra in _repository.RoomAssignments
                                         where roomIds.Contains(ra.Id)
                                         select ra).ToListAsync();

            var bookingsWithColors = from booking in bookings
                                     join color in colorAssignments on booking.BookingNumber.ToString() equals color.BookingId into gj
                                     from color in gj.DefaultIfEmpty()
                                     select new { Booking = booking, Color = color };

            var bookingsWithGuestsCount = bookingsWithColors
                .Select(join => new BookingResponse<uint>(
                    id: join.Booking.BookingNumber.ToString(),
                    name: $"{join.Booking.FirstName} {join.Booking.LastName}",
                    lastModified: join.Booking.LastModified,
                    from: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Arrival).First().Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                    to: DateTime.ParseExact(join.Booking.Rooms.OrderBy(room => room.Departure).Last().Departure, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                    deposit: join.Booking.Deposit,
                    depositConfirmed: join.Booking.DepositConfirmed,
                    isBankTransfer: join.Booking.PaymentMethod == BookingPaymentMethod.BT)
                {
                    Status = join.Booking.Status,
                    Color = join.Color?.Color,
                    Tiles = (from room in @join.Booking.Rooms
                             join assignment in roomAssignments on $"{room.StayId}-{room.Arrival}-{room.Departure}" equals assignment.Id into gj
                             from assignment in gj.DefaultIfEmpty()
                             select new { Room = room, Assignment = assignment })
                                     .ToList()
                                     .Select(x => new TileResponse<uint>(
                                        id: $"{x.Room.StayId}-{x.Room.Arrival}-{x.Room.Departure}",
                                        from: DateTime.ParseExact(x.Room.Arrival, "yyyyMMdd", null).ToString("yyyy-MM-dd"),
                                        nights: Convert.ToUInt32((DateTime.ParseExact(x.Room.Departure, "yyyyMMdd", null) - DateTime.ParseExact(x.Room.Arrival, "yyyyMMdd", null)).Days),
                                        roomType: x.Room.RoomName,
                                        rateId: x.Room.RateId,
                                        persons: Convert.ToUInt32(x.Room.Guests.Count))
                                     {
                                         RoomId = x.Assignment?.RoomId
                                     })
                                     .ToList()
                })
                .ToList();

            return bookingsWithGuestsCount;
        }
    }
}
