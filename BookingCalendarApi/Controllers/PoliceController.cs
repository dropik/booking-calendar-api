﻿using BookingCalendarApi.Models.Requests;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/police")]
    [ApiController]
    [Authorize]
    public class PoliceController : ControllerBase
    {
        private readonly IPoliceService _service;

        public PoliceController(IPoliceService service)
        {
            _service = service;
        }

        [HttpGet("ricevuta")]
        public async Task<IActionResult> GetRicevuta(string date)
        {
            var result = await _service.GetRicevuta(date);
            return File(result.Pdf, "application/pdf", result.FileName);
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestAsync(PoliceSendRequest request)
        {
            await _service.Test(request);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SendAsync(PoliceSendRequest request)
        {
            await _service.Send(request);
            return Ok();
        }

        [HttpGet("provinces")]
        public async Task<ActionResult<List<string>>> GetProvinces()
        {
            return Ok(await _service.GetProvinces());
        }
    }
}
