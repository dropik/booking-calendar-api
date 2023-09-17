using BookingCalendarApi.Models.Requests.Users;
using BookingCalendarApi.Models.Responses;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IUserService
    {
        Task<CreatedResult> Create(CreateUserRequest request);
        Task<UserResponse> GetCurrentUser();
        Task<UserResponse> Get(long id);
        Task UpdateVisibleName(UpdateVisibleNameRequest request);
        Task UpdatePassword(UpdatePasswordRequest request);
    }
}
