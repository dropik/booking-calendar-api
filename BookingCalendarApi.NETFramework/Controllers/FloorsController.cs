﻿using BookingCalendarApi.NETFramework.Filters;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Services;
using System.Threading.Tasks;
using System.Web.Http;

namespace BookingCalendarApi.Controllers
{
    [RoutePrefix("api/v1/floors")]
    [JwtAuthentication]
    public class FloorsController : ApiController
    {
        private readonly IFloorsService _service;

        public FloorsController(IFloorsService service)
        {
            _service = service;
        }

        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            return Ok(await _service.GetAll());
        }

        [HttpGet]
        [Route("{id}", Name = "GetFloor")]
        public async Task<IHttpActionResult> Get(long id)
        {
            return Ok(await _service.Get(id));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Post(Floor floor)
        {
            var result = await _service.Create(floor);
            return CreatedAtRoute("GetFloor", new { id = result.Id }, result);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IHttpActionResult> Put(long id, Floor floor)
        {
            return Ok(await _service.Update(id, floor));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IHttpActionResult> Delete(long id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}
