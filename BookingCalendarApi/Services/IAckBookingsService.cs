using BookingCalendarApi.Models;

namespace BookingCalendarApi.Services
{
    public interface IAckBookingsService
    {
        Task Ack(AckBookingsRequest request);
    }
}
