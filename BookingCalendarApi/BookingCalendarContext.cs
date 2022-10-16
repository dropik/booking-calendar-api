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
        public DbSet<TileAssignment> TileAssignments => Set<TileAssignment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Floor>()
                .HasMany(f => f.Rooms)
                .WithOne()
                .HasForeignKey(r => r.FloorId)
                .HasPrincipalKey(r => r.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TileAssignment>()
                .HasOne<Room>()
                .WithMany()
                .HasForeignKey(a => a.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
