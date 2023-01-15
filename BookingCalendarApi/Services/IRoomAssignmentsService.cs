namespace BookingCalendarApi.Services
{
    public interface IRoomAssignmentsService
    {
        Task AssignRooms(IDictionary<string, long?> assignmentRequests);
    }
}
