using System.Net;
using System.Net.Http.Json;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.RoomRates;
using BookingCalendarApi.Repository;

using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.IntegrationTests
{
    public class AssignmentsTests : ApiTest
    {
        public AssignmentsTests(ApiFixture fixture) : base(fixture) { }

        private const string TileA = "1-20230601-20230605";
        private const string TileB = "2-20230605-20230608";
        private const string TileC = "3-20230603-20230606";

        private async Task<(Structure Structure, HttpClient Client, long Room1, long Room2)> SetUpAsync(string name = "hotel")
        {
            Iperbooking.RoomRates.Add(new RoomRateRoom("Doppia"));
            if (!Iperbooking.Bookings.Any())
            {
                Iperbooking.Bookings.Add(IperbookingData.Booking(10, IperbookingData.Room(1, "20230601", "20230605")));
                Iperbooking.Bookings.Add(IperbookingData.Booking(20, IperbookingData.Room(2, "20230605", "20230608")));
                Iperbooking.Bookings.Add(IperbookingData.Booking(30, IperbookingData.Room(3, "20230603", "20230606")));
            }
            var (structure, client) = await StructureWithUserAsync(name);
            var floor = (await (await client.PostAsJsonAsync("/api/v1/floors", new { name = "Ground" })).Content.ReadFromJsonAsync<FloorDto>(Json))!;
            var room1 = (await (await client.PostAsJsonAsync("/api/v1/rooms", new { floorId = floor.Id, number = "1", type = "Doppia" })).Content.ReadFromJsonAsync<RoomDto>(Json))!;
            var room2 = (await (await client.PostAsJsonAsync("/api/v1/rooms", new { floorId = floor.Id, number = "2", type = "Doppia" })).Content.ReadFromJsonAsync<RoomDto>(Json))!;
            return (structure, client, room1.Id, room2.Id);
        }

        private async Task<Dictionary<string, long>> StoredRoomsAsync()
        {
            await using var context = Fixture.CreateContext();
            return await context.RoomAssignments.ToDictionaryAsync(a => a.Id, a => a.RoomId);
        }

        [Fact]
        public async Task Colors_and_rooms_are_stored_for_the_structure()
        {
            var (structure, client, room1, room2) = await SetUpAsync();

            var response = await client.PostAsJsonAsync("/api/v1/assignments", new
            {
                colors = new Dictionary<string, string> { ["10"] = "#ff0000" },
                rooms = new Dictionary<string, long?> { [TileA] = room1, [TileB] = room1, [TileC] = room2 },
            });

            response.EnsureSuccessStatusCode();
            (await StoredRoomsAsync()).Should().Equal(new Dictionary<string, long> { [TileA] = room1, [TileB] = room1, [TileC] = room2 });
            await using var context = Fixture.CreateContext();
            var color = await context.ColorAssignments.SingleAsync();
            (color.BookingId, color.Color, color.StructureId).Should().Be(("10", "#ff0000", structure.Id));
            (await context.RoomAssignments.Select(a => a.StructureId).Distinct().ToListAsync()).Should().Equal(structure.Id);
            Iperbooking.Requests.Where(r => r.Path == "GetBookings.cfm").Should().ContainSingle()
                .Which.Query.Should().Contain("arrivalfrom", "20230502").And.Contain("arrivalto", "20230708");
        }

        [Fact]
        public async Task Colors_and_rooms_can_be_changed_and_removed()
        {
            var (_, client, room1, room2) = await SetUpAsync();
            await client.PostAsJsonAsync("/api/v1/assignments", new
            {
                colors = new Dictionary<string, string> { ["10"] = "#ff0000" },
                rooms = new Dictionary<string, long?> { [TileA] = room1, [TileB] = room2 },
            });

            var response = await client.PostAsJsonAsync("/api/v1/assignments", new
            {
                colors = new Dictionary<string, string> { ["10"] = "#00ff00" },
                rooms = new Dictionary<string, long?> { [TileA] = room2, [TileB] = null },
            });

            response.EnsureSuccessStatusCode();
            (await StoredRoomsAsync()).Should().Equal(new Dictionary<string, long> { [TileA] = room2 });
            await using var context = Fixture.CreateContext();
            (await context.ColorAssignments.SingleAsync()).Color.Should().Be("#00ff00");
        }

        [Fact]
        public async Task Colliding_tiles_are_rejected_and_nothing_is_stored()
        {
            var (_, client, room1, _) = await SetUpAsync();

            var response = await client.PostAsJsonAsync("/api/v1/assignments", new
            {
                colors = new Dictionary<string, string> { ["10"] = "#ff0000" },
                rooms = new Dictionary<string, long?> { [TileA] = room1, [TileC] = room1 },
            });

            await ShouldBeErrorAsync(response, HttpStatusCode.BadRequest, BCError.ROOMS_COLLISION);
            (await StoredRoomsAsync()).Should().BeEmpty();
            await using var context = Fixture.CreateContext();
            (await context.ColorAssignments.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task Assignments_of_cancelled_bookings_are_released()
        {
            var (_, client, room1, room2) = await SetUpAsync();
            await client.PostAsJsonAsync("/api/v1/assignments", new { rooms = new Dictionary<string, long?> { [TileC] = room1 } });
            Iperbooking.Bookings.Single(b => b.BookingNumber == 30).Status = BookingStatus.Cancelled;

            var response = await client.PostAsJsonAsync("/api/v1/assignments", new { rooms = new Dictionary<string, long?> { [TileA] = room1 } });

            response.EnsureSuccessStatusCode();
            (await StoredRoomsAsync()).Should().Equal(new Dictionary<string, long> { [TileA] = room1 });
        }

        [Fact]
        public async Task Assignments_of_another_structure_are_not_visible()
        {
            var (_, mine, myRoom, _) = await SetUpAsync("mine");
            var (_, theirs, theirRoom, _) = await SetUpAsync("theirs");
            await mine.PostAsJsonAsync("/api/v1/assignments", new { rooms = new Dictionary<string, long?> { [TileA] = myRoom } });

            // the same tile in the other structure does not collide with mine
            var response = await theirs.PostAsJsonAsync("/api/v1/assignments", new { rooms = new Dictionary<string, long?> { [TileC] = theirRoom } });
            response.EnsureSuccessStatusCode();

            var bookings = await theirs.GetFromJsonAsync<List<BookingDto>>("/api/v1/bookings?from=2023-06-01&to=2023-06-02", Json);
            bookings!.SelectMany(b => b.Tiles).Where(t => t.Id == TileA).Should().OnlyContain(t => t.RoomId == null);
        }
    }
}
