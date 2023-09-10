using BookingCalendarApi.Repository.NETFramework.Configurations;
using MySql.Data.EntityFramework;
using System.Data.Entity;

namespace BookingCalendarApi.Repository.NETFramework
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class BookingCalendarContext : DbContext
    {
        public BookingCalendarContext() { }
        public BookingCalendarContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

        public DbSet<Structure> Structures => Set<Structure>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
        public DbSet<Nation> Nations => Set<Nation>();
        public DbSet<Floor> Floors => Set<Floor>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomAssignment> RoomAssignments => Set<RoomAssignment>();
        public DbSet<ColorAssignment> ColorAssignments => Set<ColorAssignment>();

        protected override void OnModelCreating(DbModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Configurations.Add(new StructureConfiguration());
            builder.Configurations.Add(new UserConfiguration());
            builder.Configurations.Add(new UserRefreshTokenConfiguration());
            builder.Configurations.Add(new NationConfiguration());
            builder.Configurations.Add(new FloorConfiguration());
            builder.Configurations.Add(new RoomConfiguration());
            builder.Configurations.Add(new RoomAssignmentConfiguration());
            builder.Configurations.Add(new ColorAssignmentConfiguration());
        }
    }
}
