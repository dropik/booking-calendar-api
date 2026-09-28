using System.Net;
using System.Net.Http.Json;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.IntegrationTests
{
    public class UsersTests : ApiTest
    {
        public UsersTests(ApiFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Admin_creates_a_user_who_can_then_log_in()
        {
            var structure = await Fixture.SeedStructureAsync("hotel");
            var admin = await ClientAsync(ApiFixture.MasterUsername, ApiFixture.MasterPassword);

            var response = await admin.PostAsJsonAsync("/api/v1/users", new { structureId = structure.Id, username = "new@hotel.it", password = "secret", visibleName = "New" });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var id = (await ReadJsonAsync(response))["id"]!.GetValue<long>();
            response.Headers.Location!.AbsolutePath.Should().Be($"/api/v1/users/{id}");

            var created = await admin.GetFromJsonAsync<UserResponseDto>($"/api/v1/users/{id}", Json);
            created!.Username.Should().Be("new@hotel.it");
            created.VisibleName.Should().Be("New");

            await using var context = Fixture.CreateContext();
            var user = await context.Users.SingleAsync(u => u.Id == id);
            (user.StructureId, user.IsAdmin).Should().Be((structure.Id, false));
            (await LoginAsync("new@hotel.it", "secret")).AccessToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Creating_a_user_with_an_existing_username_is_a_bad_request()
        {
            var admin = await ClientAsync(ApiFixture.MasterUsername, ApiFixture.MasterPassword);

            var response = await admin.PostAsJsonAsync("/api/v1/users", new { structureId = -1, username = ApiFixture.MasterUsername, password = "x" });

            await ShouldBeErrorAsync(response, HttpStatusCode.BadRequest, BCError.DUPLICATE_DATA);
        }

        [Fact]
        public async Task Only_admins_can_create_or_read_users()
        {
            var (_, client) = await StructureWithUserAsync();

            (await client.PostAsJsonAsync("/api/v1/users", new { structureId = -1, username = "x@hotel.it", password = "x" }))
                .StatusCode.Should().Be(HttpStatusCode.Forbidden);
            (await client.GetAsync("/api/v1/users/-1")).StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Current_user_contains_structure_floors_and_room_types()
        {
            var (structure, client) = await StructureWithUserAsync("sunrise");
            Iperbooking.RoomRates.Add(new RoomRateRoom("Doppia")
            {
                RateGroups = new List<RateGroup> { new() { Rates = new List<Rate> { new("D1", 1, 2, "BB"), new("D2", 2, 2, "HB") } } },
            });
            var floor = (await (await client.PostAsJsonAsync("/api/v1/floors", new { name = "Ground" })).Content.ReadFromJsonAsync<FloorDto>(Json))!;
            await client.PostAsJsonAsync("/api/v1/rooms", new { floorId = floor.Id, number = "2", type = "Doppia" });
            await client.PostAsJsonAsync("/api/v1/rooms", new { floorId = floor.Id, number = "1", type = "Doppia" });

            var current = await client.GetFromJsonAsync<UserResponseDto>("/api/v1/users/current", Json);

            current!.Username.Should().Be("sunrise@hotel.it");
            current.VisibleName.Should().Be("sunrise");
            current.Structure.Should().Be("sunrise");
            current.RoomTypes.Select(t => (t.Name, t.MinOccupancy, t.MaxOccupancy)).Should().Equal(("Doppia", 1u, 2u));
            current.RoomRates.Select(r => r.RateId).Should().Equal("D1", "D2");
            current.Floors.Should().ContainSingle().Which.Rooms.Select(r => r.Number).Should().Equal("2", "1");
            Iperbooking.Requests.Should().NotBeEmpty().And.OnlyContain(r => r.Host == "iperbooking.test");
            Iperbooking.Requests.Should().OnlyContain(r => r.Query["username"] == "sunrise-ib-user" && r.Query["password"] == "sunrise-ib-pwd" && r.Query["idhotel"] == "4321");
        }

        [Fact]
        public async Task Current_user_is_returned_even_when_iperbooking_is_down()
        {
            var (_, client) = await StructureWithUserAsync();
            Iperbooking.FailWith = HttpStatusCode.ServiceUnavailable;

            var current = await client.GetFromJsonAsync<UserResponseDto>("/api/v1/users/current", Json);

            current!.RoomTypes.Should().BeEmpty();
        }

        [Fact]
        public async Task Visible_name_and_password_can_be_changed()
        {
            var (_, client) = await StructureWithUserAsync();

            (await client.PatchAsync("/api/v1/users/current/visible-name", JsonContent.Create(new { visibleName = "Reception" }))).EnsureSuccessStatusCode();
            (await client.PatchAsync("/api/v1/users/current/password", JsonContent.Create(new { oldPassword = "password", newPassword = "new-password" }))).EnsureSuccessStatusCode();

            (await client.GetFromJsonAsync<UserResponseDto>("/api/v1/users/current", Json))!.VisibleName.Should().Be("Reception");
            (await Anonymous().PostAsJsonAsync("/api/v1/auth/token", new { username = "hotel@hotel.it", password = "password" })).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
            (await LoginAsync("hotel@hotel.it", "new-password")).AccessToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Changing_password_with_a_wrong_old_password_is_a_bad_request()
        {
            var (_, client) = await StructureWithUserAsync();

            var response = await client.PatchAsync("/api/v1/users/current/password", JsonContent.Create(new { oldPassword = "wrong", newPassword = "new-password" }));

            await ShouldBeErrorAsync(response, HttpStatusCode.BadRequest, BCError.AUTHENTICATION_ERROR);
        }

        private record UserResponseDto(string Username, string VisibleName, string Structure, List<RoomTypeDto> RoomTypes, List<Rate> RoomRates, List<FloorDto> Floors);

        private record RoomTypeDto(string Name, uint MinOccupancy, uint MaxOccupancy);
    }

    public record FloorDto(long Id, string Name, List<RoomDto> Rooms);

    public record RoomDto(long Id, long FloorId, string Number, string Type);
}
