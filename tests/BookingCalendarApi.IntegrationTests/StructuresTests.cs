using System.Net.Http.Json;

using BookingCalendarApi.IntegrationTests.Infrastructure;
using BookingCalendarApi.Models.Responses;

using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.IntegrationTests
{
    public class StructuresTests : ApiTest
    {
        public StructuresTests(ApiFixture fixture) : base(fixture) { }

        [Fact]
        public async Task Api_keys_of_the_current_structure_can_be_read_and_changed()
        {
            var (structure, client) = await StructureWithUserAsync("sunrise");
            var other = await Fixture.SeedStructureAsync("other");

            var keys = await client.GetFromJsonAsync<APIKeysResponse>("/api/v1/structures/current/api-keys", Json);
            (keys!.IperbookingUsername, keys.ASUtente, keys.C59Username, keys.C59Struttura).Should().Be(("sunrise-ib-user", "sunrise-as-user", "sunrise-c59-user", 555L));

            var update = new APIKeysResponse
            {
                IperbookingHotel = "999",
                IperbookingUsername = "ib",
                IperbookingPassword = "ib-pwd",
                ASUtente = "as",
                ASPassword = "as-pwd",
                ASWsKey = "as-key",
                C59Username = "c59",
                C59Password = "c59-pwd",
                C59Struttura = 777,
            };
            (await client.PatchAsync("/api/v1/structures/current/api-keys", JsonContent.Create(update))).EnsureSuccessStatusCode();

            (await client.GetFromJsonAsync<APIKeysResponse>("/api/v1/structures/current/api-keys", Json)).Should().BeEquivalentTo(update);
            await using var context = Fixture.CreateContext();
            (await context.Structures.SingleAsync(s => s.Id == structure.Id)).Name.Should().Be("sunrise");
            (await context.Structures.SingleAsync(s => s.Id == other.Id)).IperbookingUsername.Should().Be("other-ib-user");
        }
    }
}
