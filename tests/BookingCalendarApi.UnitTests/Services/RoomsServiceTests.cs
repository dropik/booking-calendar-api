using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.RoomRates;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class RoomsServiceTests
    {
        private readonly InMemoryRepository _repository = new();
        private readonly IIperbooking _iperbooking = Substitute.For<IIperbooking>();
        private readonly RoomsService _service;

        public RoomsServiceTests()
        {
            _repository.Seed(new Room { Id = 1, FloorId = 1, Number = "101", Type = "Doppia" });
            _repository.Seed(new Room { Id = 2, FloorId = 1, Number = "102", Type = "Singola" });
            _iperbooking.GetRoomRates().Returns(new List<RoomRateRoom> { RoomRate("Singola"), RoomRate("Doppia"), RoomRate("Tripla") });
            _service = new RoomsService(_repository, _iperbooking);
        }

        [Fact]
        public async Task GetAll_returns_every_room()
        {
            (await _service.GetAll()).Select(r => r.Number).Should().Equal("101", "102");
        }

        [Fact]
        public async Task Get_returns_the_room_by_id()
        {
            (await _service.Get(2)).Number.Should().Be("102");
        }

        [Fact]
        public async Task Create_stores_a_room_of_a_known_type()
        {
            var created = await _service.Create(new Room { FloorId = 1, Number = "103", Type = "Tripla" });

            created.Id.Should().NotBe(0);
            _repository.Stored<Room>().Should().ContainSingle(r => r.Id == created.Id && r.Number == "103" && r.Type == "Tripla");
        }

        [Fact]
        public async Task Create_rejects_room_types_unknown_to_iperbooking()
        {
            var act = () => _service.Create(new Room { FloorId = 1, Number = "103", Type = "Suite" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage("Given room type was not found on Iperbooking")
                .Which.ErrorCode.Should().Be(BCError.MISSING_ORIGIN_DATA);
            _repository.Stored<Room>().Should().HaveCount(2);
        }

        [Fact]
        public async Task Update_stores_the_changes()
        {
            var updated = await _service.Update(1, new Room { Id = 1, FloorId = 2, Number = "201", Type = "Tripla" });

            updated.Number.Should().Be("201");
            var stored = _repository.Stored<Room>().Single(r => r.Id == 1);
            (stored.FloorId, stored.Number, stored.Type).Should().Be((2L, "201", "Tripla"));
        }

        [Fact]
        public async Task Update_rejects_room_types_unknown_to_iperbooking()
        {
            var act = () => _service.Update(1, new Room { Id = 1, FloorId = 1, Number = "101", Type = "Suite" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage("Given room type was not found on Iperbooking")
                .Which.ErrorCode.Should().Be(BCError.MISSING_ORIGIN_DATA);
            _repository.Stored<Room>().Single(r => r.Id == 1).Type.Should().Be("Doppia");
        }

        [Fact]
        public async Task Update_of_unknown_room_throws_not_found()
        {
            var act = () => _service.Update(99, new Room { Id = 99, Type = "Doppia" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage("Room not found.").Which.ErrorCode.Should().Be(BCError.NOT_FOUND);
        }

        [Fact]
        public async Task Update_changing_the_id_throws()
        {
            var act = () => _service.Update(1, new Room { Id = 2, Number = "X", Type = "Doppia" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).WithMessage("Model id can not be changed.").Which.ErrorCode.Should().Be(BCError.ID_CHANGE_ATTEMPT);
            _repository.Stored<Room>().Select(r => r.Number).Should().Equal("101", "102");
        }

        [Fact]
        public async Task Delete_removes_the_room()
        {
            await _service.Delete(1);

            _repository.Stored<Room>().Select(r => r.Id).Should().Equal(2);
        }
    }
}
