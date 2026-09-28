using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.RoomRates;
using BookingCalendarApi.Models.Requests.Users;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using Microsoft.AspNetCore.Identity;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly InMemoryRepository _repository = new();
        private readonly IIperbooking _iperbooking = Substitute.For<IIperbooking>();
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repository.Seed(new Structure { Id = 3, Name = "Hotel Tre" });
            _repository.Seed(new Structure { Id = 4, Name = "Hotel Quattro" });
            _repository.Seed(new User { Id = 10, StructureId = 4, Username = "mario@hotel.it", VisibleName = "Mario", PasswordHash = Hash("mario@hotel.it", "old-secret") });
            _repository.Seed(new User { Id = 11, StructureId = 3, Username = "luigi@hotel.it", VisibleName = "Luigi" });
            _iperbooking.GetRoomRates().Returns(new List<RoomRateRoom>());
            _service = new UserService(TestPrincipals.Provider("mario@hotel.it", 4), _repository, _iperbooking);
        }

        private static string Hash(string username, string password) => new PasswordHasher<string>().HashPassword(username, password);

        private static bool Verifies(User user, string password)
            => new PasswordHasher<string>().VerifyHashedPassword(user.Username, user.PasswordHash, password) == PasswordVerificationResult.Success;

        private User StoredUser(string username) => _repository.Stored<User>().Single(u => u.Username == username);

        [Fact]
        public async Task Create_stores_a_non_admin_user_with_hashed_password()
        {
            var result = await _service.Create(new CreateUserRequest { StructureId = 3, Username = "new@hotel.it", Password = "pwd", VisibleName = "New" });

            var user = StoredUser("new@hotel.it");
            result.Id.Should().Be(user.Id).And.NotBe(0);
            user.StructureId.Should().Be(3);
            user.VisibleName.Should().Be("New");
            user.IsAdmin.Should().BeFalse();
            user.PasswordHash.Should().NotBe("pwd");
            Verifies(user, "pwd").Should().BeTrue();
        }

        [Fact]
        public async Task Create_without_request_throws()
        {
            var act = () => _service.Create(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null, "pwd")]
        [InlineData(" ", "pwd")]
        [InlineData("new@hotel.it", null)]
        [InlineData("new@hotel.it", " ")]
        public async Task Create_requires_username_and_password(string? username, string? password)
        {
            var act = () => _service.Create(new CreateUserRequest { StructureId = 3, Username = username, Password = password });

            await act.Should().ThrowAsync<ArgumentException>();
            _repository.Stored<User>().Should().HaveCount(2);
        }

        [Fact]
        public async Task Create_with_existing_username_throws_duplicate_data()
        {
            var act = () => _service.Create(new CreateUserRequest { StructureId = 3, Username = "luigi@hotel.it", Password = "pwd" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.DUPLICATE_DATA);
            _repository.Stored<User>().Should().HaveCount(2);
        }

        [Fact]
        public async Task GetCurrentUser_returns_user_structure_and_room_data()
        {
            _iperbooking.GetRoomRates().Returns(new List<RoomRateRoom>
            {
                new RoomRateRoom("Doppia")
                {
                    RateGroups = new List<RateGroup>
                    {
                        new() { Rates = new List<Rate> { new("D1", 1, 2, "BB"), new("D2", 2, 3, "HB") } },
                        new() { Rates = new List<Rate> { new("D3", 4, 5, "FB") } },
                    },
                },
                RoomRate("Singola", ("S1", 1, 1)),
            });

            var response = await _service.GetCurrentUser();

            response.Username.Should().Be("mario@hotel.it");
            response.VisibleName.Should().Be("Mario");
            response.Structure.Should().Be("Hotel Quattro");
            response.RoomTypes.Select(t => (t.Name, t.MinOccupancy, t.MaxOccupancy)).Should().Equal(("Doppia", 1u, 2u), ("Singola", 1u, 1u));
            response.RoomRates.Select(r => r.RateId).Should().Equal("D1", "D2", "D3", "S1");
        }

        [Fact]
        public async Task GetCurrentUser_returns_floors_and_rooms_ordered_by_id()
        {
            _repository.Seed(new Floor { Id = 2, Name = "Second" });
            _repository.Seed(new Floor { Id = 1, Name = "First" });
            _repository.Seed(new Room { Id = 6, FloorId = 1, Number = "102" });
            _repository.Seed(new Room { Id = 5, FloorId = 1, Number = "101" });
            _repository.Seed(new Room { Id = 7, FloorId = 2, Number = "201" });

            var response = await _service.GetCurrentUser();

            response.Floors.Select(f => f.Name).Should().Equal("First", "Second");
            response.Floors[0].Rooms.Select(r => r.Number).Should().Equal("101", "102");
            response.Floors[1].Rooms.Select(r => r.Number).Should().Equal("201");
        }

        [Fact]
        public async Task GetCurrentUser_ignores_iperbooking_failures()
        {
            _repository.Seed(new Floor { Id = 1, Name = "First" });
            _iperbooking.GetRoomRates().Returns<List<RoomRateRoom>>(_ => throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, "down"));

            var response = await _service.GetCurrentUser();

            response.RoomTypes.Should().BeEmpty();
            response.RoomRates.Should().BeEmpty();
            response.Floors.Should().ContainSingle();
        }

        [Fact]
        public async Task GetCurrentUser_for_unknown_user_throws_not_found()
        {
            var service = new UserService(TestPrincipals.Provider("ghost@hotel.it", 4), _repository, _iperbooking);

            var act = () => service.GetCurrentUser();

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.NOT_FOUND);
        }

        [Fact]
        public async Task Get_returns_the_user_by_id()
        {
            var response = await _service.Get(11);

            response.Username.Should().Be("luigi@hotel.it");
            response.VisibleName.Should().Be("Luigi");
        }

        [Fact]
        public async Task UpdateVisibleName_changes_the_current_user_only()
        {
            await _service.UpdateVisibleName(new UpdateVisibleNameRequest { VisibleName = "Super Mario" });

            StoredUser("mario@hotel.it").VisibleName.Should().Be("Super Mario");
            StoredUser("luigi@hotel.it").VisibleName.Should().Be("Luigi");
        }

        [Fact]
        public async Task UpdateVisibleName_for_unknown_user_throws_not_found()
        {
            var service = new UserService(TestPrincipals.Provider("ghost@hotel.it", 4), _repository, _iperbooking);

            var act = () => service.UpdateVisibleName(new UpdateVisibleNameRequest { VisibleName = "X" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.NOT_FOUND);
        }

        [Fact]
        public async Task UpdatePassword_replaces_the_password_hash()
        {
            await _service.UpdatePassword(new UpdatePasswordRequest { OldPassword = "old-secret", NewPassword = "new-secret" });

            var user = StoredUser("mario@hotel.it");
            Verifies(user, "new-secret").Should().BeTrue();
            Verifies(user, "old-secret").Should().BeFalse();
        }

        [Fact]
        public async Task UpdatePassword_with_wrong_old_password_throws_and_keeps_the_password()
        {
            var act = () => _service.UpdatePassword(new UpdatePasswordRequest { OldPassword = "wrong", NewPassword = "new-secret" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.AUTHENTICATION_ERROR);
            Verifies(StoredUser("mario@hotel.it"), "old-secret").Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePassword_requires_a_new_password()
        {
            var act = () => _service.UpdatePassword(new UpdatePasswordRequest { OldPassword = "old-secret", NewPassword = null });

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.PARAMETER_REQUIRED);
            Verifies(StoredUser("mario@hotel.it"), "old-secret").Should().BeTrue();
        }

        [Fact]
        public async Task UpdatePassword_without_request_throws()
        {
            var act = () => _service.UpdatePassword(null);

            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async Task UpdatePassword_for_unknown_user_throws_not_found()
        {
            var service = new UserService(TestPrincipals.Provider("ghost@hotel.it", 4), _repository, _iperbooking);

            var act = () => service.UpdatePassword(new UpdatePasswordRequest { OldPassword = "a", NewPassword = "b" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.NOT_FOUND);
        }
    }
}
