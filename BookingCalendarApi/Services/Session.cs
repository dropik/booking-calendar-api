using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Formats.Cbor;

namespace BookingCalendarApi.Services
{
    public class Session : ISession
    {
        private readonly BookingCalendarContext _context;

        public Guid Id { get; private set; }
        private List<SessionTile> Tiles { get; set; } = new List<SessionTile>();
        private SessionEntry? Entry { get; set; }

        public Session(BookingCalendarContext context)
        {
            _context = context;
        }

        public async Task OpenAsync(string? sessionId)
        {
            Id = GetGuid(sessionId);
            Tiles = new List<SessionTile>();
            Entry = await _context.Sessions.SingleOrDefaultAsync(session => session.Id.Equals(Id));
            if (Entry != null && !Entry.Id.Equals(Guid.Empty))
            {
                var data = Entry.Data;
                var reader = new CborReader(data);
                reader.ReadStartArray();
                while (reader.PeekState() != CborReaderState.EndArray)
                {
                    var tileId = reader.ReadTextString();
                    var lastModified = reader.ReadTextString();
                    Tiles.Add(new SessionTile(tileId, lastModified));
                }
            }
        }

        private Guid GetGuid(string? sessionId)
        {
            try
            {
                return sessionId != null ? Guid.Parse(sessionId) : Guid.NewGuid();
            } catch (Exception)
            {
                return Guid.NewGuid();
            }
        }
        
        public IEnumerable<FlattenedRoom> ExcludeRooms(IEnumerable<FlattenedRoom> rooms)
        {
            foreach (var room in rooms)
            {
                var query = Tiles.Where(session => session.TileId == room.Id);
                if (query.Any())
                {
                    var tile = query.First();
                    if (tile.LastModified == room.Booking.LastModified)
                    {
                        continue;
                    }
                    tile.LastModified = room.Booking.LastModified;
                } else
                {
                    Tiles.Add(new SessionTile(room.Id, room.Booking.LastModified));
                }

                yield return room;
            }
        }

        public void Close()
        {
            var writer = new CborWriter();
            writer.WriteStartArray(Tiles.Count * 2);
            foreach (var tile in Tiles)
            {
                writer.WriteTextString(tile.TileId);
                writer.WriteTextString(tile.LastModified);
            }
            writer.WriteEndArray();
            var data = writer.Encode();

            if (Entry != null && !Entry.Id.Equals(Guid.Empty))
            {
                Entry.Data = data;
            }
            else
            {
                _context.Sessions.Add(new SessionEntry(Id) { Data = data });
            }
        }
    }
}