using BookingCalendarApi.Models.Iperbooking;
using BookingCalendarApi.Models.Iperbooking.Bookings;
using BookingCalendarApi.Models.Iperbooking.Guests;
using BookingCalendarApi.Models.Iperbooking.RoomRates;
using Humanizer;
using System.Text;
using System.Text.Json;

namespace BookingCalendarApi.Services
{
    public class Iperbooking : IIperbooking
    {
        private readonly Auth _auth;

        public Iperbooking(IConfiguration configuration)
        {
            _auth = configuration.GetSection("Iperbooking").Get<Auth>();
        }

        public async Task<IEnumerable<RoomRateRoom>> GetRoomRatesAsync()
        {
            var url = $"https://api.iperbooking.net/v1/GetRoomRates.cfm?idhotel={_auth.IdHotel}&username={_auth.Username}&password={_auth.Password}&format=json";
            try
            {
                using HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync(url);
                using HttpContent content = response.Content;

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
                    } else
                    {
                        throw new Exception("Cannot deserialize fetched iperbooking data");
                    }
                } else
                {
                    throw new Exception("Fetched empty data from iperbooking");
                }
            } catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return new List<RoomRateRoom>();
        }

        public async Task<List<Booking>> GetBookingsAsync(string from, string to, bool exactPeriod = false)
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

            var url = $"https://api.iperbooking.net/v1/GetBookings.cfm?idhotel={_auth.IdHotel}&username={_auth.Username}&password={_auth.Password}&format=json&arrivalfrom={arrivalFrom}&arrivalto={arrivalTo}";
            try
            {
                using HttpClient client = new();
                using HttpResponseMessage response = await client.GetAsync(url);
                using HttpContent content = response.Content;

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
                    } else
                    {
                        throw new Exception("Cannot deserialize fetched iperbooking data");
                    }
                } else
                {
                    throw new Exception("Fetched empty data from iperbooking");
                }
            } catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return new List<Booking>();
        }

        public async Task<GuestsResponse> GetGuestsAsync(string reservationId)
        {
            try
            {
                var requestData = new Models.Iperbooking.Guests.GuestsRequest(int.Parse(_auth.IdHotel), _auth.Username, _auth.Password, reservationId);
                var json = JsonSerializer.Serialize(requestData);
                var body = new StringContent(json, Encoding.UTF8, "application/json");
                var url = "https://api.iperbooking.net/v1/GetGuests.cfm";
                
                using HttpClient client = new();
                using var response = await client.PostAsync(url, body);
                using var content = response.Content;
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
                    } else
                    {
                        throw new Exception("Cannot deserialize fetched iperbooking data");
                    }
                }
                else
                {
                    throw new Exception("Fetched empty data from iperbooking");
                }

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            return new GuestsResponse();
        }
    }
}
