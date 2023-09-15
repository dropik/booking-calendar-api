using BookingCalendarApi.Models.Responses;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IUserService
    {
        Task<CurrentUserResponse> GetCurrentUser();
    }
}
