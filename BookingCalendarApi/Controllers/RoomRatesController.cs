using BookingCalendarApi.Models;
using BookingCalendarApi.Models.Iperbooking.RoomRates;
using BookingCalendarApi.Models.Responses;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/room-rates")]
    [ApiController]
    public class RoomRatesController : ControllerBase
    {
        private readonly IIperbooking iperbooking;

        public RoomRatesController(IIperbooking iperbooking)
        {
            this.iperbooking = iperbooking;
        }

        [HttpGet]
        public async Task<ActionResult<RoomRatesResponse>> GetAsync()
        {
            var roomRates = await iperbooking.GetRoomRatesAsync();
            return Ok(new RoomRatesResponse()
            {
                RoomTypes = roomRates
                    .SelectRoomTypes()
                    .ToList(),

                RoomRates = roomRates
                    .SelectMany(rate => rate.RateGroups)
                    .SelectMany(group => group.Rates)
                    .ToList()
            });
        }
    }

    static class Extensions
    {
        public static IEnumerable<RoomType> SelectRoomTypes(this IEnumerable<RoomRateRoom> roomRates)
        {
            return roomRates
                .SelectMany(
                    roomRate => roomRate.RateGroups.Take(1),
                    (roomRate, rateGroup) => new { name = roomRate.RoomName, rateGroup }
                )
                .SelectMany(
                    roomRate => roomRate.rateGroup.Rates.Take(1),
                    (roomRate, rate) => new RoomType(roomRate.name, rate.MinOccupancy, rate.MaxOccupancy)
                );
        }
    }
}
