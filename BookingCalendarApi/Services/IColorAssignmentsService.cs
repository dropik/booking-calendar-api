namespace BookingCalendarApi.Services
{
    public interface IColorAssignmentsService
    {
        Task AssignColors(IDictionary<string, string> colors);
    }
}
