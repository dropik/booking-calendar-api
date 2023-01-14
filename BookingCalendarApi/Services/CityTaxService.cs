using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class CityTaxService : ICityTaxService
    {
        private readonly IAssignedBookingComposer _assignedBookingComposer;
        private readonly IIperbooking _iperbooking;
        private readonly Func<string, string, IEnumerable<Reservation>, ICityTaxCalculator> _calculatorProvider;
        private readonly DataContext _dataContext;
        private readonly BookingCalendarContext _context;

        public CityTaxService(
            IAssignedBookingComposer assignedBookingComposer,
            IIperbooking iperbooking,
            Func<string, string, IEnumerable<Reservation>, ICityTaxCalculator> calculatorProvider,
            DataContext dataContext,
            BookingCalendarContext context)
        {
            _assignedBookingComposer = assignedBookingComposer;
            _iperbooking = iperbooking;
            _calculatorProvider = calculatorProvider;
            _dataContext = dataContext;
            _context = context;
        }

        public async Task<CityTaxResponse> Get(string from, string to)
        {
            _dataContext.RoomAssignments.AddRange(await _context.RoomAssignments.ToListAsync());

            var bookings = (await _iperbooking.GetBookingsAsync(from, to))
                .ExcludeCancelled()
                .SelectInRange(from, to);

            var bookingIds = "";
            foreach (var booking in bookings)
            {
                bookingIds += $"{booking.BookingNumber},";
            }

            var guestResponse = await _iperbooking.GetGuestsAsync(bookingIds);

            var calculator = _calculatorProvider(from, to, guestResponse.Reservations);

            return bookings
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
                .ExcludeNotAssigned()
                .UseCalculator(calculator);
        }
    }
}
