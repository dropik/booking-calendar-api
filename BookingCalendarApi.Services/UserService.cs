using BookingCalendarApi.Models.Configurations;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Requests.Users;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Extensions;

using Microsoft.AspNetCore.Identity;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class UserService : IUserService
    {
        private readonly IUserClaimsProvider _userClaimsProvider;
        private readonly IRepository _repository;
        private readonly IIperbooking _iperbooking;

        public UserService(IUserClaimsProvider userClaimsProvider, IRepository repository, IIperbooking iperbooking)
        {
            _userClaimsProvider = userClaimsProvider;
            _repository = repository;
            _iperbooking = iperbooking;
        }

        public async Task<CreatedResult> Create(CreateUserRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.Username))
            {
                throw new ArgumentException("Username must not be empty", nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                throw new ArgumentException("Password must not be empty", nameof(request));
            }

            var existingUser = await _repository.Users.SingleOrDefaultAsync(u => u.Username == request.Username);
            if (existingUser != null)
            {
                throw new BookingCalendarException(BCError.DUPLICATE_DATA, "A user with following username already exists");
            }

            var passwordHasher = new PasswordHasher<string>();
            var passwordHash = passwordHasher.HashPassword(request.Username, request.Password);
            var user = new User()
            {
                StructureId = request.StructureId,
                Username = request.Username,
                PasswordHash = passwordHash,
                VisibleName = request.VisibleName,
                IsAdmin = false,
            };
            user = _repository.Add(user);
            await _repository.SaveChangesAsync();
            return new CreatedResult
            {
                Id = user.Id,
            };
        }

        public async Task<UserResponse> GetCurrentUser()
        {
            var result = new UserResponse();

            var username = _userClaimsProvider.User.Identity?.Name ?? "";
            var user = await _repository.Users.SingleOrDefaultAsync(u => u.Username == username)
                ?? throw new BookingCalendarException(BCError.NOT_FOUND, "Unable to find current user");

            result.Username = user.Username;
            result.VisibleName = user.VisibleName;

            var structureId = long.Parse(_userClaimsProvider.User.Claims.FirstOrDefault(c => c.Type == JWT.STRUCTURE_CLAIM)?.Value ?? "0");
            var structure = await _repository.Structures.SingleAsync(s => s.Id == structureId);
            result.Structure = structure.Name;

            try
            {
                var roomRates = await _iperbooking.GetRoomRates();
                result.RoomTypes = roomRates
                    .SelectMany(
                        roomRate => roomRate.RateGroups.Take(1),
                        (roomRate, rateGroup) => new { name = roomRate.RoomName, rateGroup })
                    .SelectMany(
                        roomRate => roomRate.rateGroup.Rates.Take(1),
                        (roomRate, rate) => new RoomType(roomRate.name, rate.MinOccupancy, rate.MaxOccupancy))
                    .ToList();
                result.RoomRates = roomRates
                    .SelectMany(rate => rate.RateGroups)
                    .SelectMany(group => group.Rates)
                    .ToList();
            }
            catch (Exception) { }

            result.Floors = await _repository.Floors.Include(floor => floor.Rooms).ToListAsync();
            result.Floors = result.Floors
                .Select(f =>
                {
                    f.Rooms = f.Rooms.OrderBy(r => r.Id).ToList();
                    return f;
                })
                .OrderBy(f => f.Id)
                .ToList();

            return result;
        }

        public async Task<UserResponse> Get(long id)
        {
            var result = new UserResponse();
            var user = await _repository.Users.SingleAsync(u => u.Id == id);
            result.Username = user.Username;
            result.VisibleName = user.VisibleName;
            return result;
        }

        public async Task UpdateVisibleName(UpdateVisibleNameRequest request)
        {
            var username = _userClaimsProvider.User.Identity?.Name ?? "";
            var user = await _repository.Users.SingleOrDefaultAsync(u => u.Username == username)
                ?? throw new BookingCalendarException(BCError.NOT_FOUND, "Unable to find current user");

            user.VisibleName = request.VisibleName;

            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdatePassword(UpdatePasswordRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var username = _userClaimsProvider.User.Identity?.Name ?? "";
            var user = await _repository.Users.SingleOrDefaultAsync(u => u.Username == username)
                ?? throw new BookingCalendarException(BCError.NOT_FOUND, "Unable to find current user");

            var passwordHasher = new PasswordHasher<string>();
            var verifyOldPasswordResult = passwordHasher.VerifyHashedPassword(username, user.PasswordHash, request.OldPassword);
            if (verifyOldPasswordResult != PasswordVerificationResult.Success)
            {
                throw new BookingCalendarException(BCError.AUTHENTICATION_ERROR, "Old password is not correct");
            }

            if (request.NewPassword == null)
            {
                throw new BookingCalendarException(BCError.PARAMETER_REQUIRED, "New password must be provided");
            }

            user.PasswordHash = passwordHasher.HashPassword(username, request.NewPassword);
            _repository.Update(user);
            await _repository.SaveChangesAsync();
        }
    }
}
