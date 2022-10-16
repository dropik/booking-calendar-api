using BookingCalendarApi.Models;
using BookingCalendarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookingCalendarApi.Controllers
{
    [Route("api/v1/room-types")]
    [ApiController]
    public class RoomTypesController : ControllerBase
    {
        private readonly IIperbooking iperbooking;

        public RoomTypesController(IIperbooking iperbooking)
        {
            this.iperbooking = iperbooking;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomType>>> GetAsync()
        {
            var roomRates = await iperbooking.GetRoomRatesAsync();
            return roomRates.SelectRoomTypes().ToList();
        }
    }

    static class Extensions
    {
        public static IEnumerable<RoomType> SelectRoomTypes(this IEnumerable<Models.Iperbooking.RoomRates.Room> roomRates)
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
