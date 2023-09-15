using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Repository;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class FloorsService : IFloorsService
    {
        private readonly IRepository _repository;

        public FloorsService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Floor>> GetAll()
        {
            return await _repository.ToListAsync(_repository.Floors
                .Include(floor => floor.Rooms
                    .OrderBy(room => room.Id)));
        }

        public async Task<Floor> Get(long id)
        {
            var floor = await _repository.SingleAsync(_repository.Floors.Where(f => f.Id == id));
            return floor;
        }

        public async Task<Floor> Create(Floor floor)
        {
            _repository.Add(floor);
            await _repository.SaveChangesAsync();
            return await Get(floor.Id);
        }

        public async Task<Floor> Update(long id, Floor floor)
        {
            if (!await _repository.AnyAsync(_repository.Floors.Where(f => f.Id == id)))
            {
                throw new BookingCalendarException(BCError.NOT_FOUND, "Floor not found.");
            }

            if (id != floor.Id)
            {
                throw new BookingCalendarException(BCError.ID_CHANGE_ATTEMPT, "Model id can not be changed.");
            }

            _repository.Update(floor);
            await _repository.SaveChangesAsync();
            return await Get(floor.Id);
        }

        public async Task Delete(long id)
        {
            var floor = await Get(id);
            _repository.Remove(floor);
            await _repository.SaveChangesAsync();
        }
    }
}
