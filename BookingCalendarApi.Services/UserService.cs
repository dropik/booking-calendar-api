using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class UserService : IUserService
    {
        private readonly ClaimsPrincipal _user;
        private readonly IRepository _repository;
        private readonly IIperbooking _iperbooking;

        public UserService(ClaimsPrincipal user, IRepository repository, IIperbooking iperbooking)
        {
            _user = user;
            _repository = repository;
            _iperbooking = iperbooking;
        }

        public async Task<CurrentUserResponse> GetCurrentUser()
        {
            var result = new CurrentUserResponse();

            var username = _user.Identity?.Name ?? "";
            var user = await _repository.SingleOrDefaultAsync(_repository.Users.Where(u => u.Username == username))
                ?? throw new BookingCalendarException(BCError.NOT_FOUND, "Unable to find current user");

            result.Username = user.Username;
            result.VisibleName = user.VisibleName;

            var roomRates = await _iperbooking.GetRoomRates();
            result.RoomTypes = roomRates
                .SelectMany(
                    roomRate => roomRate.RateGroups.Take(1),
                    (roomRate, rateGroup) => new { name = roomRate.RoomName, rateGroup })
                .SelectMany(
                    roomRate => roomRate.rateGroup.Rates.Take(1),
                    (roomRate, rate) => new RoomTypeResponse(roomRate.name, rate.MinOccupancy, rate.MaxOccupancy))
                .ToList();

            result.RoomRates = roomRates
                .SelectMany(rate => rate.RateGroups)
                .SelectMany(group => group.Rates)
                .ToList();

            result.Floors = await _repository.ToListAsync(_repository.Floors
                .Include(floor => floor.Rooms
                    .OrderBy(room => room.Id)));

            return result;
        }
    }
}
