using BookingCalendarApi.Exceptions;
using BookingCalendarApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class FloorsService : IFloorsService
    {
        private readonly BookingCalendarContext _context;

        public FloorsService(BookingCalendarContext context)
        {
            _context = context;
        }

        public async Task<List<Floor>> GetAll()
        {
            return await _context.Floors
                .Include(floor => floor.Rooms
                    .OrderBy(room => room.Id))
                .ToListAsync();
        }

        public async Task<Floor> Get(long id)
        {
            var floor = await _context.Floors.SingleAsync(floor => floor.Id == id);
            return floor;
        }

        public async Task<Floor> Create(Floor floor)
        {
            _context.Floors.Add(floor);
            await _context.SaveChangesAsync();
            return await Get(floor.Id);
        }

        public async Task<Floor> Update(long id, Floor floor)
        {
            if (! await _context.Floors.AnyAsync(floor => floor.Id == id))
            {
                throw new BookingCalendarException(BCError.NOT_FOUND, "Floor not found.");
            }

            if (id != floor.Id)
            {
                throw new BookingCalendarException(BCError.ID_CHANGE_ATTEMPT, "Model id can not be changed.");
            }

            _context.Attach(floor);
            _context.Entry(floor).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _context.Entry(floor).State = EntityState.Detached;
            return await Get(floor.Id);
        }

        public async Task Delete(long id)
        {
            var floor = await Get(id);
            _context.Floors.Remove(floor);
            await _context.SaveChangesAsync();
        }
    }
}
