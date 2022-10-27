using C59Service;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IstatController : ControllerBase
    {
        private readonly EC59ServiceEndpoint _service;
        private readonly IConfiguration _configuration;
        private readonly string _username;
        private readonly string _password;

        public IstatController(EC59ServiceEndpoint service, IConfiguration configuration)
        {
            _service = service;
            _configuration = configuration;
            var c59 = _configuration.GetSection("C59Service");
            _username = c59.GetValue<string>("Username");
            _password = c59.GetValue<string>("Password");
        }

        [HttpGet]
        public async Task<ActionResult<listaStruttureResponse>> GetAsync()
        {
            var request = new listaStrutture(_username, _password);
            var response = await _service.listaStruttureAsync(request);
            return response;
        }
    }
}
