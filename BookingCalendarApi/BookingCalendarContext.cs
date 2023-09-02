using BookingCalendarApi.ModelConfigurations;
using BookingCalendarApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi
{
    public class BookingCalendarContext : DbContext
    {
        public BookingCalendarContext(DbContextOptions<BookingCalendarContext> options) : base(options) { }

        public DbSet<Structure> Structures => Set<Structure>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
        public DbSet<Nation> Nations => Set<Nation>();
        public DbSet<Floor> Floors => Set<Floor>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<SessionEntry> Sessions => Set<SessionEntry>();
        public DbSet<RoomAssignment> RoomAssignments => Set<RoomAssignment>();
        public DbSet<ColorAssignment> ColorAssignments => Set<ColorAssignment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new StructureConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserRefreshTokenConfiguration());
            builder.ApplyConfiguration(new NationConfiguration());
            builder.ApplyConfiguration(new FloorConfiguration());
            builder.ApplyConfiguration(new RoomConfiguration());
            builder.ApplyConfiguration(new SessionEntryConfiguration());
            builder.ApplyConfiguration(new RoomAssignmentConfiguration());
            builder.ApplyConfiguration(new ColorAssignmentConfiguration());
        }
    }
}
