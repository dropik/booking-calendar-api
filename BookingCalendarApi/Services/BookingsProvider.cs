using BookingCalendarApi.Models.Iperbooking.Bookings;

namespace BookingCalendarApi.Services
{
    public class BookingsProvider : IBookingsProvider
    {
        private readonly IIperbooking _iperbooking;

        public BookingsProvider(IIperbooking iperbooking)
        {
            _iperbooking = iperbooking;
        }

        public IEnumerable<Booking> Bookings { get; private set; } = new List<Booking>();

        public async Task FetchBookingsAsync(string from, string to, bool exactPeriod = false)
        {
            var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
            var toDate = DateTime.ParseExact(to, "yyyy-MM-dd", null);

            if (!exactPeriod)
            {
                fromDate = fromDate.AddDays(-30);
                toDate = toDate.AddDays(30);
            }

            var arrivalFrom = fromDate.ToString("yyyyMMdd");
            var arrivalTo = toDate.ToString("yyyyMMdd");

            Bookings = await _iperbooking.GetBookingsAsync(arrivalFrom, arrivalTo);
        }
    }
}
