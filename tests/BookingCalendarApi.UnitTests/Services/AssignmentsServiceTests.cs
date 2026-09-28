using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using BookingCalendarApi.UnitTests.Infrastructure;

using static BookingCalendarApi.UnitTests.Infrastructure.Fixtures;

namespace BookingCalendarApi.UnitTests.Services
{
    public class AssignmentsServiceTests
    {
        private readonly InMemoryRepository _repository = new();
        private readonly IIperbooking _iperbooking = Substitute.For<IIperbooking>();
        private readonly AssignmentsService _service;

        private static readonly string TileA = TileId(1, "20230601", "20230605");
        private static readonly string TileB = TileId(2, "20230605", "20230608");
        private static readonly string TileC = TileId(3, "20230603", "20230606");

        public AssignmentsServiceTests()
        {
            _iperbooking.GetBookings(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<Booking>
            {
                Booking(10, Room(1, "20230601", "20230605")),
                Booking(20, Room(2, "20230605", "20230608")),
                Booking(30, Room(3, "20230603", "20230606")),
            });
            _service = new AssignmentsService(_repository, _iperbooking);
        }

        private Dictionary<string, long> StoredRooms() => _repository.Stored<RoomAssignment>().ToDictionary(a => a.Id, a => a.RoomId);

        private static AssignmentsRequest Rooms(params (string Tile, long? Room)[] rooms)
            => new() { Colors = new Dictionary<string, string>(), Rooms = rooms.ToDictionary(r => r.Tile, r => r.Room) };

        [Fact]
        public async Task Null_request_does_nothing()
        {
            await _service.Set(null);

            _repository.SaveChangesCalls.Should().Be(0);
        }

        [Fact]
        public async Task Missing_colors_and_rooms_are_treated_as_empty()
        {
            await _service.Set(new AssignmentsRequest { Colors = null, Rooms = null });

            _repository.SaveChangesCalls.Should().Be(1);
            _repository.Stored<ColorAssignment>().Should().BeEmpty();
            _repository.Stored<RoomAssignment>().Should().BeEmpty();
            await _iperbooking.DidNotReceiveWithAnyArgs().GetBookings(default!, default!, default);
        }

        [Fact]
        public async Task Colors_are_added_and_updated()
        {
            _repository.Seed(new ColorAssignment { BookingId = "10", Color = "red" });

            await _service.Set(new AssignmentsRequest { Colors = new() { ["10"] = "green", ["20"] = "blue" } });

            _repository.Stored<ColorAssignment>().ToDictionary(c => c.BookingId, c => c.Color)
                .Should().Equal(new Dictionary<string, string> { ["10"] = "green", ["20"] = "blue" });
        }

        [Fact]
        public async Task Bookings_are_fetched_for_the_period_covered_by_the_tiles()
        {
            await _service.Set(Rooms((TileB, 1), (TileA, 2)));

            await _iperbooking.Received(1).GetBookings("2023-06-01", "2023-06-08", false);
        }

        [Fact]
        public async Task Rooms_are_assigned_moved_and_unassigned()
        {
            _repository.Seed(new RoomAssignment { Id = TileA, RoomId = 1 });
            _repository.Seed(new RoomAssignment { Id = TileB, RoomId = 2 });

            await _service.Set(Rooms((TileA, 3), (TileB, null), (TileC, 4)));

            StoredRooms().Should().Equal(new Dictionary<string, long> { [TileA] = 3, [TileC] = 4 });
        }

        [Fact]
        public async Task Unassigning_a_tile_without_assignment_does_nothing()
        {
            await _service.Set(Rooms((TileA, null)));

            StoredRooms().Should().BeEmpty();
        }

        [Fact]
        public async Task Tiles_unknown_to_iperbooking_are_still_assigned()
        {
            var unknown = TileId(99, "20230610", "20230612");

            await _service.Set(Rooms((unknown, 5)));

            StoredRooms().Should().Equal(new Dictionary<string, long> { [unknown] = 5 });
        }

        [Fact]
        public async Task Assignments_of_cancelled_bookings_are_removed()
        {
            _iperbooking.GetBookings(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<Booking>
            {
                Booking(10, Room(1, "20230601", "20230605")),
                Booking(20, "A", "B", BookingStatus.Cancelled, Room(2, "20230605", "20230608")),
            });
            _repository.Seed(new RoomAssignment { Id = TileB, RoomId = 1 });

            await _service.Set(Rooms((TileA, 2)));

            StoredRooms().Should().Equal(new Dictionary<string, long> { [TileA] = 2 });
        }

        [Fact]
        public async Task Cancelled_bookings_do_not_collide()
        {
            _iperbooking.GetBookings(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>()).Returns(new List<Booking>
            {
                Booking(10, Room(1, "20230601", "20230605")),
                Booking(30, "A", "B", BookingStatus.Cancelled, Room(3, "20230603", "20230606")),
            });
            _repository.Seed(new RoomAssignment { Id = TileC, RoomId = 1 });

            await _service.Set(Rooms((TileA, 1)));

            StoredRooms().Should().Equal(new Dictionary<string, long> { [TileA] = 1 });
        }

        [Fact]
        public async Task Overlapping_tiles_in_the_same_room_are_rejected_and_nothing_is_saved()
        {
            var act = () => _service.Set(new AssignmentsRequest
            {
                Colors = new() { ["10"] = "red" },
                Rooms = new() { [TileA] = 1, [TileC] = 1 },
            });

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.ROOMS_COLLISION);
            _repository.SaveChangesCalls.Should().Be(0);
            _repository.Stored<RoomAssignment>().Should().BeEmpty();
            _repository.Stored<ColorAssignment>().Should().BeEmpty();
        }

        [Fact]
        public async Task A_tile_colliding_with_an_existing_assignment_is_rejected()
        {
            _repository.Seed(new RoomAssignment { Id = TileA, RoomId = 1 });

            var act = () => _service.Set(Rooms((TileC, 1)));

            (await act.Should().ThrowAsync<BookingCalendarException>()).Which.ErrorCode.Should().Be(BCError.ROOMS_COLLISION);
            StoredRooms().Should().Equal(new Dictionary<string, long> { [TileA] = 1 });
        }

        [Fact]
        public async Task Back_to_back_tiles_in_the_same_room_are_allowed_in_any_order()
        {
            await _service.Set(Rooms((TileB, 1), (TileA, 1)));

            StoredRooms().Should().Equal(new Dictionary<string, long> { [TileA] = 1, [TileB] = 1 });
        }

        [Fact]
        public async Task Overlapping_tiles_in_different_rooms_are_allowed()
        {
            await _service.Set(Rooms((TileA, 1), (TileC, 2)));

            StoredRooms().Should().Equal(new Dictionary<string, long> { [TileA] = 1, [TileC] = 2 });
        }

        [Fact]
        public async Task Moving_an_assigned_tile_away_frees_its_room()
        {
            _repository.Seed(new RoomAssignment { Id = TileA, RoomId = 1 });

            await _service.Set(Rooms((TileA, 2), (TileC, 1)));

            StoredRooms().Should().Equal(new Dictionary<string, long> { [TileA] = 2, [TileC] = 1 });
        }

        [Fact]
        public async Task Unassigning_a_tile_frees_its_room()
        {
            _repository.Seed(new RoomAssignment { Id = TileA, RoomId = 1 });

            await _service.Set(Rooms((TileA, null), (TileC, 1)));

            StoredRooms().Should().Equal(new Dictionary<string, long> { [TileC] = 1 });
        }
    }
}
