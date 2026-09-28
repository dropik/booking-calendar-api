using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.Bookings;

using static BookingCalendarApi.IntegrationTests.Infrastructure.IperbookingData;

namespace BookingCalendarApi.IntegrationTests
{
    /// <summary>Bookings, clients and city tax: Iperbooking data combined with the assignments stored in the database.</summary>
    public class BookingsTests : ApiTest
    {
        public BookingsTests(ApiFixture fixture) : base(fixture) { }

        private const string MarioTile = "1-20230610-20230613";
        private const string LuigiTile = "2-20230612-20230614";

        private async Task<(HttpClient Client, long RoomId)> SetUpAsync()
        {
            var mario = Booking(42, "Mario", "Rossi", Room(1, "20230610", "20230613", guests: 2));
            mario.Status = BookingStatus.Modified;
            mario.PaymentMethod = BookingPaymentMethod.BT;
            mario.Deposit = 100;
            Iperbooking.Bookings.Add(mario);
            Iperbooking.Bookings.Add(Booking(43, "Luigi", "Verdi", Room(2, "20230612", "20230614")));
            Iperbooking.Bookings.Add(Booking(44, "Anna", "Neri", Room(3, "20230801", "20230805")));
            Iperbooking.Reservations.Add(Reservation(42,
                Guest("7", 1, "Mario", "Rossi", "19800115", g => { g.BirthCountry = "IT"; g.BirthCounty = "RM"; g.BirthCity = "Roma"; }),
                Guest("8", 1, "Luca", "Rossi", "20150301")));
            Iperbooking.Reservations.Add(Reservation(43, Guest("9", 2, "Luigi", "Verdi", "19700101")));

            var (structure, client) = await StructureWithUserAsync();
            var room = await Fixture.SeedRoomAsync(structure.Id, MarioTile);
            (await client.PostAsJsonAsync("/api/v1/assignments", new { colors = new Dictionary<string, string> { ["42"] = "#ff0000" } })).EnsureSuccessStatusCode();
            return (client, room.Id);
        }

        [Fact]
        public async Task Bookings_of_a_period_come_with_colors_and_assigned_rooms()
        {
            var (client, roomId) = await SetUpAsync();

            var response = await client.GetAsync("/api/v1/bookings?from=2023-06-10&to=2023-06-11");

            response.EnsureSuccessStatusCode();
            var json = await ReadJsonAsync(response);
            json[0]!["status"]!.GetValue<string>().Should().Be("modified");
            json[0]!["isBankTransfer"]!.GetValue<bool>().Should().BeTrue();
            json[0]!["deposit"]!.GetValue<decimal>().Should().Be(100);
            var bookings = json.Deserialize<List<BookingDto>>(Json)!;
            bookings.Select(b => (b.Id, b.Name, b.From, b.To, b.Color)).Should().Equal(
                ("42", "Mario Rossi", "2023-06-10", "2023-06-13", "#ff0000"),
                ("43", "Luigi Verdi", "2023-06-12", "2023-06-14", null));
            bookings[0].Tiles.Should().ContainSingle().Which.Should().Be(new TileDto(MarioTile, "2023-06-10", 3, "Doppia", roomId));
            json[0]!["tiles"]![0]!["persons"]!.GetValue<int>().Should().Be(2);
            bookings[1].Tiles.Single().RoomId.Should().BeNull();
            // nulls are left out of the responses
            json[1]!.AsObject().ContainsKey("color").Should().BeFalse();
            json[1]!["tiles"]![0]!.AsObject().ContainsKey("roomId").Should().BeFalse();
            Iperbooking.Requests.Single(r => r.Path == "GetBookings.cfm").Query
                .Should().Contain("arrivalfrom", "20230511").And.Contain("arrivalto", "20230711");
        }

        [Fact]
        public async Task A_booking_comes_with_its_guests()
        {
            var (client, roomId) = await SetUpAsync();

            var response = await client.GetAsync("/api/v1/bookings/42?from=2023-06-10");

            response.EnsureSuccessStatusCode();
            var json = await ReadJsonAsync(response);
            json["name"]!.GetValue<string>().Should().Be("Mario Rossi");
            json["color"]!.GetValue<string>().Should().Be("#ff0000");
            var tile = json["tiles"]![0]!;
            tile["roomId"]!.GetValue<long>().Should().Be(roomId);
            tile["persons"]!.AsArray().Select(p => (p!["name"]!.GetValue<string>(), p["dateOfBirth"]!.GetValue<string>()))
                .Should().Equal(("Mario", "1980-01-15"), ("Luca", "2015-03-01"));
            tile["persons"]![0]!["placeOfBirth"]!.GetValue<string>().Should().Be("Roma");
        }

        [Fact]
        public async Task An_unknown_booking_is_not_found()
        {
            var (client, _) = await SetUpAsync();

            await ShouldBeErrorAsync(await client.GetAsync("/api/v1/bookings/99?from=2023-06-10"), HttpStatusCode.NotFound, BCError.NOT_FOUND);
        }

        [Fact]
        public async Task Bookings_can_be_searched_by_name()
        {
            var (client, _) = await SetUpAsync();

            var json = await ReadJsonAsync(await client.GetAsync("/api/v1/bookings/by-name?from=2023-06-01&to=2023-06-30&name=verdi"));

            json.AsArray().Should().ContainSingle();
            json[0]!["id"]!.GetValue<string>().Should().Be("43");
            json[0]!["occupations"]!.GetValue<int>().Should().Be(1);
        }

        [Fact]
        public async Task Clients_can_be_searched_by_name_and_by_tile()
        {
            var (client, _) = await SetUpAsync();

            var byQuery = await ReadJsonAsync(await client.GetAsync("/api/v1/clients/by-query?query=rossi&from=2023-06-01&to=2023-06-30"));
            var byTile = await ReadJsonAsync(await client.GetAsync($"/api/v1/clients/by-tile?bookingId=42&tileId={MarioTile}"));

            byQuery.AsArray().Select(c => (c!["name"]!.GetValue<string>(), c["bookingName"]!.GetValue<string>(), c["bookingFrom"]!.GetValue<string>()))
                .Should().Equal(("Mario", "Mario Rossi", "2023-06-10"), ("Luca", "Mario Rossi", "2023-06-10"));
            byTile.AsArray().Select(c => c!["id"]!.GetValue<string>()).Should().Equal("7", "8");
        }

        [Fact]
        public async Task City_tax_counts_the_nights_of_assigned_guests()
        {
            var (client, _) = await SetUpAsync();

            var json = await ReadJsonAsync(await client.GetAsync("/api/v1/city-tax?from=2023-06-01&to=2023-06-30"));

            // only Mario's stay is assigned: 3 nights for him, 3 for his child
            (json["standard"]!.GetValue<int>(), json["children"]!.GetValue<int>(), json["over10Days"]!.GetValue<int>()).Should().Be((3, 3, 0));
        }

        [Fact]
        public async Task Iperbooking_failures_are_bad_requests()
        {
            var (client, _) = await SetUpAsync();
            Iperbooking.FailWith = HttpStatusCode.InternalServerError;

            await ShouldBeErrorAsync(await client.GetAsync("/api/v1/bookings?from=2023-06-10&to=2023-06-11"), HttpStatusCode.BadRequest, BCError.IPERBOOKING_ERROR);
        }

        [Fact]
        public async Task Iperbooking_being_unreachable_is_a_request_timeout()
        {
            var (client, _) = await SetUpAsync();
            Iperbooking.Unreachable = true;

            await ShouldBeErrorAsync(await client.GetAsync("/api/v1/bookings?from=2023-06-10&to=2023-06-11"), HttpStatusCode.RequestTimeout, BCError.CONNECTION_ERROR);
        }
    }
}
