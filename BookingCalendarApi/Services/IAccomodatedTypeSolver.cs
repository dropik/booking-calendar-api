using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public interface IAccomodatedTypeSolver
    {
        public void Solve(TrackedRecord record, IEnumerable<TrackedRecord> recordsBlock);
    }
}
