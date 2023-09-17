using BookingCalendarApi.Models.Exceptions;
using BookingCalendarApi.Repository;
using BookingCalendarApi.Repository.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingCalendarApi.Services
{
    public class RoomsService : IRoomsService
    {
        private readonly IRepository _repository;
        private readonly IIperbooking _iperbooking;

        public RoomsService(IRepository repository, IIperbooking iperbooking)
        {
            _repository = repository;
            _iperbooking = iperbooking;
        }

        public async Task<List<Room>> GetAll()
        {
            return await _repository.Rooms.ToListAsync();
        }

        public async Task<Room> Get(long id)
        {
            return await _repository.Rooms.SingleAsync(room => room.Id == id);
        }

        public async Task<Room> Create(Room room)
        {
            await CheckRoomTypeExists(room.Type);

            _repository.Add(room);
            await _repository.SaveChangesAsync();
            return await Get(room.Id);
        }

        public async Task<Room> Update(long id, Room room)
        {
            if (!await _repository.Rooms.AnyAsync(r => r.Id == id))
            {
                throw new BookingCalendarException(BCError.NOT_FOUND, "Room not found.");
            }

            if (id != room.Id)
            {
                throw new BookingCalendarException(BCError.ID_CHANGE_ATTEMPT, "Model id can not be changed.");
            }

            await CheckRoomTypeExists(room.Type);

            _repository.Update(room);
            await _repository.SaveChangesAsync();
            return await Get(room.Id);
        }

        public async Task Delete(long id)
        {
            var room = await Get(id);
            _repository.Remove(room);
            await _repository.SaveChangesAsync();
        }

        private async Task CheckRoomTypeExists(string roomType)
        {
            var roomRates = await _iperbooking.GetRoomRates();
            var matchedTypes = roomRates.Where(r => r.RoomName == roomType);

            if (!matchedTypes.Any())
            {
                throw new BookingCalendarException(BCError.MISSING_ORIGIN_DATA, "Given room type was not found on Iperbooking");
            }
        }
    }
}
