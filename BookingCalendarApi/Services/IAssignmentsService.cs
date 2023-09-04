using BookingCalendarApi.Models.Requests;

namespace BookingCalendarApi.Services
{
    public interface IAssignmentsService
    {
        Task Set(AssignmentsRequest request);
        Task SetColors(IDictionary<string, string> request);
    }
}
