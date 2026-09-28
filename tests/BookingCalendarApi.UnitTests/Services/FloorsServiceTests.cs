using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

namespace BookingCalendarApi.UnitTests.Services
{
    public class FloorsServiceTests
    {
        private readonly InMemoryRepository _repository = new();
        private readonly FloorsService _service;

        public FloorsServiceTests()
        {
            _repository.Seed(new Floor { Id = 1, Name = "Ground" });
            _repository.Seed(new Floor { Id = 2, Name = "First" });
            _repository.Seed(new Room { Id = 10, FloorId = 2, Number = "101" });
            _service = new FloorsService(_repository);
        }

        [Fact]
        public async Task GetAll_returns_floors_with_their_rooms()
        {
            var floors = await _service.GetAll();

            floors.Select(f => f.Name).Should().Equal("Ground", "First");
            floors[1].Rooms.Select(r => r.Number).Should().Equal("101");
        }

        [Fact]
        public async Task Get_returns_the_floor_by_id()
        {
            (await _service.Get(2)).Name.Should().Be("First");
        }

        [Fact]
        public async Task Create_stores_the_floor_and_returns_it_with_its_id()
        {
            var created = await _service.Create(new Floor { Name = "Attic" });

            created.Id.Should().NotBe(0);
            created.Name.Should().Be("Attic");
            _repository.Stored<Floor>().Should().ContainSingle(f => f.Id == created.Id && f.Name == "Attic");
        }

        [Fact]
        public async Task Update_stores_the_changes()
        {
            var updated = await _service.Update(2, new Floor { Id = 2, Name = "Renamed" });

            updated.Name.Should().Be("Renamed");
            _repository.Stored<Floor>().Single(f => f.Id == 2).Name.Should().Be("Renamed");
        }

        [Fact]
        public async Task Update_of_unknown_floor_throws_not_found()
        {
            var act = () => _service.Update(99, new Floor { Id = 99, Name = "X" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.NOT_FOUND);
        }

        [Fact]
        public async Task Update_changing_the_id_throws_and_changes_nothing()
        {
            var act = () => _service.Update(2, new Floor { Id = 1, Name = "Renamed" });

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.ID_CHANGE_ATTEMPT);
            _repository.Stored<Floor>().Select(f => f.Name).Should().Equal("Ground", "First");
        }

        [Fact]
        public async Task Delete_removes_the_floor()
        {
            await _service.Delete(1);

            _repository.Stored<Floor>().Select(f => f.Id).Should().Equal(2);
        }
    }
}
