﻿using BookingCalendarApi.Models.AlloggiatiService;

namespace BookingCalendarApi.Services
{
    public interface IAlloggiatiTableReader
    {
        public List<Place> ReadAsPlaces(string data);
        public List<DocumentType> ReadAsDocumentTypes(string data);
        public List<AccomodatedType> ReadAsAccomodatedTypes(string data);
    }
}
