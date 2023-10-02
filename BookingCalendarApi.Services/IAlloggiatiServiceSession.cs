using BookingCalendarApi.Models.AlloggiatiService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public interface IAlloggiatiServiceSession
    {
        Task Open();
        Task SendData(IList<string> data, bool test);
        Task<byte[]> GetRicevuta(DateTime date);
        Task<List<Place>> GetPlaces();
    }
}
