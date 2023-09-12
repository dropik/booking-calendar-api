using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Services;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [RoutePrefix("api/v1/police")]
    [JwtAuthentication]
    public class PoliceController : ApiController
    {
        private readonly IPoliceService _service;

        public PoliceController(IPoliceService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("ricevuta")]
        public async Task<IHttpActionResult> GetRicevuta(string date)
        {
            var result = await _service.GetRicevuta(date);
            ActionContext.Response.StatusCode = System.Net.HttpStatusCode.OK;
            ActionContext.Response.Content = new StreamContent(new MemoryStream(result.Pdf));
            ActionContext.Response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            ActionContext.Response.Content.Headers.ContentLength = result.Pdf.Length;
            if (ContentDispositionHeaderValue.TryParse($"inline; filename={result.FileName}", out ContentDispositionHeaderValue contentDisposition))
            {
                ActionContext.Response.Content.Headers.ContentDisposition = contentDisposition;
            }
            return ResponseMessage(ActionContext.Response);
        }

        [HttpPost]
        [Route("test")]
        public async Task<IHttpActionResult> TestAsync(PoliceSendRequest request)
        {
            await _service.Test(request);
            return Ok();
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendAsync(PoliceSendRequest request)
        {
            await _service.Send(request);
            return Ok();
        }

        [HttpGet]
        [Route("provinces")]
        public async Task<IHttpActionResult> GetProvinces()
        {
            return Ok(await _service.GetProvinces());
        }
    }
}
