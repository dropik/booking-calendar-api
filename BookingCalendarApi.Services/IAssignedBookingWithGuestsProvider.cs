using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IAssignedBookingWithGuestsProvider
    {
        Task<List<AssignedBooking<Guest>>> Get(string from, string to = null, bool exactPeriod = true);
    }
}
