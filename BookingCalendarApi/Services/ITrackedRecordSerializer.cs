using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public interface ITrackedRecordSerializer
    {
        public string Serialize(TrackedRecord record);
    }
}
