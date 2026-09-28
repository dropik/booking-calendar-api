using System.Net;
using System.Text.Json;
using System.Web;

using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

namespace BookingCalendarApi.IntegrationTests.Infrastructure
{
    /// <summary>
    /// In-process stand-in for the Iperbooking REST API, plugged in as the primary handler of the
    /// Iperbooking typed HttpClient. Like the real API it returns only bookings arriving in the
    /// requested period and only the reservations that were asked for.
    /// </summary>
    public class FakeIperbooking : HttpMessageHandler
    {
        public record Request(string Host, string Path, IDictionary<string, string> Query, JsonElement? Body);

        private readonly object _lock = new();

        public List<RoomRateRoom> RoomRates { get; } = new();
        public List<Booking> Bookings { get; } = new();
        public List<Reservation> Reservations { get; } = new();
        public List<Request> Requests { get; } = new();
        public HttpStatusCode? FailWith { get; set; }
        public bool Unreachable { get; set; }

        public void Reset()
        {
            lock (_lock)
            {
                RoomRates.Clear();
                Bookings.Clear();
                Reservations.Clear();
                Requests.Clear();
                FailWith = null;
                Unreachable = false;
            }
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var uri = request.RequestUri!;
            var parsedQuery = HttpUtility.ParseQueryString(uri.Query);
            var query = parsedQuery.AllKeys.ToDictionary(key => key!, key => parsedQuery[key]!);
            JsonElement? body = request.Content == null
                ? null
                : JsonDocument.Parse(await request.Content.ReadAsStringAsync(cancellationToken)).RootElement.Clone();
            var path = uri.AbsolutePath.Split('/').Last();

            lock (_lock)
            {
                Requests.Add(new Request(uri.Host, path, query, body));

                if (Unreachable)
                {
                    throw new HttpRequestException("connection refused");
                }
                if (FailWith != null)
                {
                    return new HttpResponseMessage(FailWith.Value);
                }

                return path switch
                {
                    "GetRoomRates.cfm" => Json(RoomRates),
                    "GetBookings.cfm" => Json(Bookings
                        .Where(booking => booking.Rooms.Any(room =>
                            string.CompareOrdinal(room.Arrival, query["arrivalfrom"]) >= 0 &&
                            string.CompareOrdinal(room.Arrival, query["arrivalto"]) <= 0))
                        .ToList()),
                    "GetGuests.cfm" => Json(new GuestsResponse
                    {
                        Reservations = Reservations
                            .Where(r => body!.Value.GetProperty("ReservationId").GetString()!.Split(',').Contains(r.ReservationId.ToString()))
                            .ToList(),
                    }),
                    _ => new HttpResponseMessage(HttpStatusCode.NotFound),
                };
            }
        }

        private static HttpResponseMessage Json<T>(T value) => new(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(value)),
        };
    }
}
