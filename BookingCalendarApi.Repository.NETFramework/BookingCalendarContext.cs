using BookingCalendarApi.Repository.NETFramework.Configurations;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

namespace BookingCalendarApi.Repository.NETFramework
{
    public class BookingCalendarContext : DbContext
    {
        public BookingCalendarContext() { }

        public DbSet<Structure> Structures => Set<Structure>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>();
        public DbSet<Nation> Nations => Set<Nation>();
        public DbSet<Floor> Floors => Set<Floor>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<RoomAssignment> RoomAssignments => Set<RoomAssignment>();
        public DbSet<ColorAssignment> ColorAssignments => Set<ColorAssignment>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseMySql(
                ConfigurationManager.AppSettings["DB_ConnectionString"]);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new StructureConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new UserRefreshTokenConfiguration());
            builder.ApplyConfiguration(new NationConfiguration());
            builder.ApplyConfiguration(new FloorConfiguration());
            builder.ApplyConfiguration(new RoomConfiguration());
            builder.ApplyConfiguration(new RoomAssignmentConfiguration());
            builder.ApplyConfiguration(new ColorAssignmentConfiguration());
        }
    }
}
