using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Repository;

using ApiEnvironment = BookingCalendarApi.Models.Configurations.Environment;

namespace BookingCalendarApi.IntegrationTests.Infrastructure
{
    [Collection(ApiCollection.Name)]
    public abstract class ApiTest : IAsyncLifetime
    {
        protected static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

        protected ApiTest(ApiFixture fixture)
        {
            Fixture = fixture;
        }

        protected ApiFixture Fixture { get; }
        protected FakeIperbooking Iperbooking => Fixture.Iperbooking;

        public Task InitializeAsync() => Fixture.ResetAsync();

        public Task DisposeAsync() => Task.CompletedTask;

        protected HttpClient Anonymous(ApiEnvironment environment = ApiEnvironment.Production) => Fixture.Factory(environment).CreateClient();

        protected async Task<TokenResponse> LoginAsync(string username, string password, ApiEnvironment environment = ApiEnvironment.Production)
        {
            var response = await Anonymous(environment).PostAsJsonAsync("/api/v1/auth/token", new { username, password });
            response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());
            return (await response.Content.ReadFromJsonAsync<TokenResponse>(Json))!;
        }

        protected async Task<HttpClient> ClientAsync(string username, string password, ApiEnvironment environment = ApiEnvironment.Production)
        {
            var tokens = await LoginAsync(username, password, environment);
            var client = Anonymous(environment);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            return client;
        }

        /// <summary>Creates a structure with a (non admin) user and returns a client authenticated as that user.</summary>
        protected async Task<(Structure Structure, HttpClient Client)> StructureWithUserAsync(
            string name = "hotel", ApiEnvironment environment = ApiEnvironment.Production, bool isAdmin = false)
        {
            var structure = await Fixture.SeedStructureAsync(name);
            await Fixture.SeedUserAsync(structure.Id, $"{name}@hotel.it", "password", isAdmin);
            return (structure, await ClientAsync($"{name}@hotel.it", "password", environment));
        }

        protected static async Task<JsonNode> ReadJsonAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            return JsonNode.Parse(body) ?? throw new InvalidOperationException($"Empty body, status {response.StatusCode}");
        }

        protected static async Task ShouldBeErrorAsync(HttpResponseMessage response, HttpStatusCode status, int errorCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            response.StatusCode.Should().Be(status, body);
            JsonNode.Parse(body)!["errorCode"]!.GetValue<int>().Should().Be(errorCode, body);
        }
    }
}
