using BookingCalendarApi.ModelConfigurations;
using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi
{
    public class BookingCalendarContext : DbContext
    {
        public BookingCalendarContext(DbContextOptions<BookingCalendarContext> options) : base(options) { }

        public DbSet<Floor> Floors => Set<Floor>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<SessionEntry> Sessions => Set<SessionEntry>();
        public DbSet<RoomAssignment> RoomAssignments => Set<RoomAssignment>();
        public DbSet<ColorAssignment> ColorAssignments => Set<ColorAssignment>();
        public DbSet<Nation> Nations => Set<Nation>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new FloorConfiguration());
            builder.ApplyConfiguration(new RoomConfiguration());
            builder.ApplyConfiguration(new SessionEntryConfiguration());
            builder.ApplyConfiguration(new RoomAssignmentConfiguration());
            builder.ApplyConfiguration(new ColorAssignmentConfiguration());
            builder.ApplyConfiguration(new NationConfiguration());
        }
    }
}
