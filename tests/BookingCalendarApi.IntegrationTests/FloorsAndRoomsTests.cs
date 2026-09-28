using System.Net;
using System.Net.Http.Json;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.IntegrationTests
{
    public class FloorsAndRoomsTests : ApiTest
    {
        public FloorsAndRoomsTests(ApiFixture fixture) : base(fixture) { }

        private void RoomTypes(params string[] names) => Iperbooking.RoomRates.AddRange(names.Select(n => new RoomRateRoom(n)));

        private static async Task<FloorDto> CreateFloorAsync(HttpClient client, string name)
        {
            var response = await client.PostAsJsonAsync("/api/v1/floors", new { name });
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            return (await response.Content.ReadFromJsonAsync<FloorDto>(Json))!;
        }

        private static async Task<RoomDto> CreateRoomAsync(HttpClient client, long floorId, string number, string type = "Doppia")
        {
            var response = await client.PostAsJsonAsync("/api/v1/rooms", new { floorId, number, type });
            response.StatusCode.Should().Be(HttpStatusCode.Created, await response.Content.ReadAsStringAsync());
            return (await response.Content.ReadFromJsonAsync<RoomDto>(Json))!;
        }

        [Fact]
        public async Task Floors_and_rooms_can_be_created_read_updated_and_deleted()
        {
            RoomTypes("Doppia", "Tripla");
            var (structure, client) = await StructureWithUserAsync();

            var floor = await CreateFloorAsync(client, "Ground");
            var room = await CreateRoomAsync(client, floor.Id, "101");

            (await client.GetFromJsonAsync<FloorDto>($"/api/v1/floors/{floor.Id}", Json))!.Name.Should().Be("Ground");
            (await client.GetFromJsonAsync<RoomDto>($"/api/v1/rooms/{room.Id}", Json))!.Number.Should().Be("101");

            (await client.PutAsJsonAsync($"/api/v1/floors/{floor.Id}", new { id = floor.Id, name = "First" })).EnsureSuccessStatusCode();
            var updated = await client.PutAsJsonAsync($"/api/v1/rooms/{room.Id}", new { id = room.Id, floorId = floor.Id, number = "102", type = "Tripla" });
            updated.EnsureSuccessStatusCode();
            (await updated.Content.ReadFromJsonAsync<RoomDto>(Json))!.Type.Should().Be("Tripla");

            var floors = await client.GetFromJsonAsync<List<FloorDto>>("/api/v1/floors", Json);
            floors!.Should().ContainSingle().Which.Name.Should().Be("First");
            floors[0].Rooms.Should().ContainSingle().Which.Number.Should().Be("102");
            (await client.GetFromJsonAsync<List<RoomDto>>("/api/v1/rooms", Json))!.Select(r => r.Type).Should().Equal("Tripla");

            await using (var context = Fixture.CreateContext())
            {
                (await context.Floors.SingleAsync()).StructureId.Should().Be(structure.Id);
                (await context.Rooms.SingleAsync()).StructureId.Should().Be(structure.Id);
            }

            (await client.DeleteAsync($"/api/v1/rooms/{room.Id}")).EnsureSuccessStatusCode();
            (await client.DeleteAsync($"/api/v1/floors/{floor.Id}")).EnsureSuccessStatusCode();
            (await client.GetFromJsonAsync<List<FloorDto>>("/api/v1/floors", Json)).Should().BeEmpty();
            (await client.GetFromJsonAsync<List<RoomDto>>("/api/v1/rooms", Json)).Should().BeEmpty();
        }

        [Fact]
        public async Task Rooms_of_a_floor_are_ordered_by_id()
        {
            RoomTypes("Doppia");
            var (_, client) = await StructureWithUserAsync();
            var floor = await CreateFloorAsync(client, "Ground");
            var first = await CreateRoomAsync(client, floor.Id, "B");
            var second = await CreateRoomAsync(client, floor.Id, "A");

            var floors = await client.GetFromJsonAsync<List<FloorDto>>("/api/v1/floors", Json);

            floors!.Single().Rooms.Select(r => r.Id).Should().Equal(first.Id, second.Id);
        }

        [Fact]
        public async Task Deleting_a_floor_deletes_its_rooms()
        {
            RoomTypes("Doppia");
            var (_, client) = await StructureWithUserAsync();
            var floor = await CreateFloorAsync(client, "Ground");
            await CreateRoomAsync(client, floor.Id, "101");

            (await client.DeleteAsync($"/api/v1/floors/{floor.Id}")).EnsureSuccessStatusCode();

            await using var context = Fixture.CreateContext();
            (await context.Rooms.CountAsync()).Should().Be(0);
        }

        [Fact]
        public async Task Room_types_must_exist_on_iperbooking()
        {
            RoomTypes("Doppia");
            var (_, client) = await StructureWithUserAsync();
            var floor = await CreateFloorAsync(client, "Ground");

            var response = await client.PostAsJsonAsync("/api/v1/rooms", new { floorId = floor.Id, number = "101", type = "Suite" });

            await ShouldBeErrorAsync(response, HttpStatusCode.BadRequest, BCError.MISSING_ORIGIN_DATA);
        }

        [Fact]
        public async Task Updating_an_unknown_floor_is_not_found()
        {
            var (_, client) = await StructureWithUserAsync();
            await CreateFloorAsync(client, "Ground");

            var response = await client.PutAsJsonAsync("/api/v1/floors/12345", new { id = 12345, name = "X" });

            await ShouldBeErrorAsync(response, HttpStatusCode.NotFound, BCError.NOT_FOUND);
        }

        [Fact]
        public async Task Changing_the_id_of_a_floor_is_a_bad_request()
        {
            var (_, client) = await StructureWithUserAsync();
            var floor = await CreateFloorAsync(client, "Ground");

            var response = await client.PutAsJsonAsync($"/api/v1/floors/{floor.Id}", new { id = floor.Id + 1, name = "X" });

            await ShouldBeErrorAsync(response, HttpStatusCode.BadRequest, BCError.ID_CHANGE_ATTEMPT);
        }

        [Fact]
        public async Task Structures_cannot_see_or_change_each_other_floors_and_rooms()
        {
            RoomTypes("Doppia");
            var (_, mine) = await StructureWithUserAsync("mine");
            var (_, theirs) = await StructureWithUserAsync("theirs");
            var floor = await CreateFloorAsync(mine, "Ground");
            var room = await CreateRoomAsync(mine, floor.Id, "101");

            (await theirs.GetFromJsonAsync<List<FloorDto>>("/api/v1/floors", Json)).Should().BeEmpty();
            (await theirs.GetFromJsonAsync<List<RoomDto>>("/api/v1/rooms", Json)).Should().BeEmpty();
            (await theirs.GetAsync($"/api/v1/floors/{floor.Id}")).IsSuccessStatusCode.Should().BeFalse();
            (await theirs.GetAsync($"/api/v1/rooms/{room.Id}")).IsSuccessStatusCode.Should().BeFalse();
            await ShouldBeErrorAsync(await theirs.PutAsJsonAsync($"/api/v1/floors/{floor.Id}", new { id = floor.Id, name = "Hacked" }), HttpStatusCode.NotFound, BCError.NOT_FOUND);
            await ShouldBeErrorAsync(await theirs.PutAsJsonAsync($"/api/v1/rooms/{room.Id}", new { id = room.Id, floorId = floor.Id, number = "666", type = "Doppia" }), HttpStatusCode.NotFound, BCError.NOT_FOUND);
            (await theirs.DeleteAsync($"/api/v1/floors/{floor.Id}")).IsSuccessStatusCode.Should().BeFalse();
            (await theirs.DeleteAsync($"/api/v1/rooms/{room.Id}")).IsSuccessStatusCode.Should().BeFalse();

            var floors = await mine.GetFromJsonAsync<List<FloorDto>>("/api/v1/floors", Json);
            floors!.Single().Name.Should().Be("Ground");
            floors[0].Rooms.Single().Number.Should().Be("101");
        }

        [Fact]
        public async Task Missing_floors_are_reported_as_server_errors()
        {
            var (_, client) = await StructureWithUserAsync();

            var response = await client.GetAsync("/api/v1/floors/12345");

            // not a BookingCalendarException: the exception filter falls back to 500 with the message
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            (await response.Content.ReadAsStringAsync()).Should().Contain("Sequence contains no elements");
        }
    }
}
