using System.Net;

namespace BookingCalendarApi.UnitTests.Infrastructure
{
    /// <summary>Answers HTTP requests with a canned response and records what was sent.</summary>
    public class StubHttpMessageHandler : HttpMessageHandler
    {
        public record RecordedRequest(HttpMethod Method, Uri Uri, string? Body, string? ContentType);

        public List<RecordedRequest> Requests { get; } = new();

        public Func<HttpRequestMessage, HttpResponseMessage> Respond { get; set; } = _ => new HttpResponseMessage(HttpStatusCode.OK);

        public static HttpResponseMessage Json(string json, HttpStatusCode status = HttpStatusCode.OK)
            => new(status) { Content = new StringContent(json) };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var body = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
            Requests.Add(new RecordedRequest(request.Method, request.RequestUri!, body, request.Content?.Headers.ContentType?.ToString()));
            return Respond(request);
        }
    }
}
