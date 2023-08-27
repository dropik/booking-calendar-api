﻿using BookingCalendarApi.Models.Entities;

namespace BookingCalendarApi.Services
{
    public interface IFloorsService
    {
        Task<List<Floor>> GetAll();
        Task<Floor> Get(long id);
        Task<Floor> Create(Floor floor);
        Task<Floor> Update(long id, Floor floor);
        Task Delete(long id);
    }
}