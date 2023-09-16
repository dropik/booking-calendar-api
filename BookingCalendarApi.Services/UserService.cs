﻿using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;

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

            var existingUser = await _repository.SingleOrDefaultAsync(_repository.Users.Where(u => u.Username == request.Username));
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

            result.Floors = await _repository.ToListAsync(_repository.Floors.Include(floor => floor.Rooms));
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
            var user = await _repository.SingleAsync(_repository.Users.Where(u => u.Id == id));
            result.Username = user.Username;
            result.VisibleName = user.VisibleName;
            return result;
        }
    }
}
