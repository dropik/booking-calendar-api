﻿using BookingCalendarApi.Models.Entities;

namespace BookingCalendarApi.Services
{
    public interface IRoomsService
    {
        Task<List<Room>> GetAll();
        Task<Room> Get(long id);
        Task<Room> Create(Room room);
        Task<Room> Update(long id, Room room);
        Task Delete(long id);
    }
}
