using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class RoomsService : IRoomsService
    {
        private readonly BookingCalendarContext _context;
        private readonly IIperbooking _iperbooking;

        public RoomsService(BookingCalendarContext context, IIperbooking iperbooking)
        {
            _context = context;
            _iperbooking = iperbooking;
        }

        public async Task<List<Room>> GetAll()
        {
            return await _context.Rooms.ToListAsync();
        }

        public async Task<Room> Get(long id)
        {
            return await _context.Rooms.SingleAsync(room => room.Id == id);
        }

        public async Task<Room> Create(Room room)
        {
            await CheckRoomTypeExists(room.Type);

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return await Get(room.Id);
        }

        public async Task<Room> Update(long id, Room room)
        {
            if (!await _context.Rooms.AnyAsync(room => room.Id == id))
            {
                throw new Exception("Room not found.");
            }

            if (id != room.Id)
            {
                throw new Exception("Model id can not be changed.");
            }

            await CheckRoomTypeExists(room.Type);

            _context.Attach(room);
            _context.Entry(room).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _context.Entry(room).State = EntityState.Detached;
            return await Get(room.Id);
        }

        public async Task Delete(long id)
        {
            var room = await Get(id);
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }

        private async Task CheckRoomTypeExists(string roomType)
        {
            var roomRates = await _iperbooking.GetRoomRates();
            var matchedTypes = roomRates.Where(r => r.RoomName == roomType);

            if (!matchedTypes.Any())
            {
                throw new Exception("Given room type was not found on Iperbooking");
            }
        }
    }
}
