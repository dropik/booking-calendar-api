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

        private const string GET_ROOM_RATES_URL = "https://api.iperbooking.net/v1/GetRoomRates.cfm?idhotel={0}&username={1}&password={2}&format=json";
        private const string GET_BOOKINGS_URL = "https://api.iperbooking.net/v1/GetBookings.cfm?idhotel={0}&username={1}&password={2}&format=json&arrivalfrom={3}&arrivalto={4}";
        private const string GET_GUESTS_URL = "https://api.Iperbooking.net/v1/GetGuests.cfm";

        private const string UNABLE_GET_DATA_MESSAGE = "Unable to get data from Iperbooking";
        private const string FAILED_ESTABLISH_CONNECTION_MESSAGE = "Failed estalish connection with Iperbooking: {0}";

        public Iperbooking(IStructureService structureService)
        {
            _structureService = structureService;
        }

        public async Task<List<RoomRateRoom>> GetRoomRates()
        {
            var auth = await _structureService.GetIperbookingCredentials();
            var url = string.Format(GET_ROOM_RATES_URL, auth.IdHotel, auth.Username, auth.Password);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                        }

                        using (HttpContent content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync()
                                ?? throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);

                            try
                            {
                                var poco = JsonSerializer.Deserialize<List<RoomRateRoom>>(data, new JsonSerializerOptions()
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                return poco ?? throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                            }
                            catch (Exception)
                            {
                                throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                            }
                        }
                    }
                }
            } catch(HttpRequestException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, string.Format(FAILED_ESTABLISH_CONNECTION_MESSAGE, exception.Message));
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
            var url = string.Format(GET_BOOKINGS_URL, auth.IdHotel, auth.Username, auth.Password, arrivalFrom, arrivalTo);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(url))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                        }

                        using (HttpContent content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync()
                                ?? throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                            try
                            {
                                var poco = JsonSerializer.Deserialize<List<Booking>>(data, new JsonSerializerOptions()
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                return poco ?? throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                            }
                            catch (Exception)
                            {
                                throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                            }
                        }
                    }
                }
            } catch(HttpRequestException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, string.Format(FAILED_ESTABLISH_CONNECTION_MESSAGE, exception.Message));
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

                using (HttpClient client = new HttpClient())
                {
                    using (var response = await client.PostAsync(GET_GUESTS_URL, body))
                    {
                        if (!response.IsSuccessStatusCode)
                        {
                            throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                        }

                        using (var content = response.Content)
                        {
                            var data = await content.ReadAsStringAsync()
                                ?? throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);

                            try
                            {
                                var poco = JsonSerializer.Deserialize<GuestsResponse>(data, new JsonSerializerOptions()
                                {
                                    PropertyNameCaseInsensitive = true
                                });
                                return poco ?? throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                            }
                            catch (Exception)
                            {
                                throw new BookingCalendarException(BCError.IPERBOOKING_ERROR, UNABLE_GET_DATA_MESSAGE);
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException exception)
            {
                throw new BookingCalendarException(BCError.CONNECTION_ERROR, string.Format(FAILED_ESTABLISH_CONNECTION_MESSAGE, exception.Message));
            }
        }
    }
}
