﻿using BookingCalendarApi.Models.AlloggiatiService;
using Sylvan.Data;
using Sylvan.Data.Csv;

namespace BookingCalendarApi.Services
{
    public class AlloggiatiTableReader : IAlloggiatiTableReader
    {
        public List<Place> ReadAsPlaces(string data)
        {
            return ReadAs<Place>(data);
        }

        private static List<T> ReadAs<T>(string data)
            where T : class, new()
        {
            using var textReader = new StringReader(data);
            using var csvReader = CsvDataReader.Create(textReader);
            return csvReader.GetRecords<T>().ToList();
        }
    }
}
