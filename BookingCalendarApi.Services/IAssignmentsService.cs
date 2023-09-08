using BookingCalendarApi.Models.Requests;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IAssignmentsService
    {
        Task Set(AssignmentsRequest request);
    }
}
