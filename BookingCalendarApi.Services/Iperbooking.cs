using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Iperbooking.RoomRates;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class Iperbooking : IIperbooking
    {
        private readonly IStructureService _structureService;

        public Iperbooking(IStructureService structureService)
        {
            _structureService = structureService;
        }

        public async Task<IEnumerable<RoomRateRoom>> GetRoomRates()
        {
            var auth = await _structureService.GetIperbookingCredentials();
            var url = $"https://api.iperbooking.net/v1/GetRoomRates.cfm?idhotel={auth.IdHotel}&username={auth.Username}&password={auth.Password}&format=json";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        using (HttpContent content = response.Content)
                        {

                            var data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                var poco = JsonSerializer.Deserialize<ICollection<RoomRateRoom>>(data, new JsonSerializerOptions()
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                if (poco != null)
                                {
                                    return poco;
                                }
                                else
                                {
                                    throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, "Cannot deserialize fetched iperbooking data");
                                }
                            }
                            else
                            {
                                throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, "Fetched empty data from iperbooking");
                            }
                        }
                    }
                }
            } catch(HttpRequestException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed estalish connection with Iperbooking: {exception.Message}");
            }
        }

        public async Task<List<Booking>> GetBookings(string from, string to, bool exactPeriod = false)
        {
            var fromDate = DateTime.ParseExact(from, "yyyy-MM-dd", null);
            var toDate = DateTime.ParseExact(to, "yyyy-MM-dd", null);

            if (!exactPeriod)
            {
                fromDate = fromDate.AddDays(-30);
                toDate = toDate.AddDays(30);
            }

            var arrivalFrom = fromDate.ToString("yyyyMMdd");
            var arrivalTo = toDate.ToString("yyyyMMdd");

            var auth = await _structureService.GetIperbookingCredentials();
            var url = $"https://api.iperbooking.net/v1/GetBookings.cfm?idhotel={auth.IdHotel}&username={auth.Username}&password={auth.Password}&format=json&arrivalfrom={arrivalFrom}&arrivalto={arrivalTo}";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        using (HttpContent content = response.Content)
                        {

                            var data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                var poco = JsonSerializer.Deserialize<List<Booking>>(data, new JsonSerializerOptions()
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                if (poco != null)
                                {
                                    return poco;
                                }
                                else
                                {
                                    throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, "Cannot deserialize fetched iperbooking data");
                                }
                            }
                            else
                            {
                                throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, "Fetched empty data from iperbooking");
                            }
                        }
                    }
                }
            } catch(HttpRequestException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed estalish connection with Iperbooking: {exception.Message}");
            }
        }

        public async Task<GuestsResponse> GetGuests(string reservationId)
        {
            try
            {
                var auth = await _structureService.GetIperbookingCredentials();
                var requestData = new GuestsRequest(int.Parse(auth.IdHotel), auth.Username, auth.Password, reservationId);
                var json = JsonSerializer.Serialize(requestData);
                var body = new StringContent(json, Encoding.UTF8, "application/json");
                var url = "https://api.iperbooking.net/v1/GetGuests.cfm";

                using (HttpClient client = new HttpClient())
                {
                    using (var response = await client.PostAsync(url, body))
                    {
                        using (var content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            if (data != null)
                            {
                                var poco = JsonSerializer.Deserialize<GuestsResponse>(data, new JsonSerializerOptions()
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                if (poco != null)
                                {
                                    return poco;
                                }
                                else
                                {
                                    throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, "Cannot deserialize fetched iperbooking data");
                                }
                            }
                            else
                            {
                                throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, "Fetched empty data from iperbooking");
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, $"Failed estalish connection with Iperbooking: {exception.Message}");
            }
        }
    }
}
