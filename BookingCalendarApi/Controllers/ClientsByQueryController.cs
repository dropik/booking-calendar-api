using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/clients-by-query")]
    [ApiController]
    public class ClientsByQueryController : ControllerBase
    {
        private readonly IIperbooking _iperbooking;

        public ClientsByQueryController(IIperbooking iperbooking)
        {
            _iperbooking = iperbooking;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetAsync(string query, string from, string to)
        {
            try
            {
                var arrivalFrom = DateTime.ParseExact(from, "yyyy-MM-dd", null).ToString("yyyyMMdd");
                var arrivalTo = DateTime.ParseExact(to, "yyyy-MM-dd", null).ToString("yyyyMMdd");
                var bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);

                string bookingIds = "";
                foreach (var booking in bookings)
                {
                    bookingIds += $"{booking.BookingNumber},";
                }

                var guestsResponse = await _iperbooking.GetGuestsAsync(bookingIds);

                return guestsResponse.Reservations
                    .SelectByQuery(query)
                    .ComposeResponse()
                    .ToList();
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }
    }
}
