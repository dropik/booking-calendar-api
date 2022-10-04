using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi
{
    public class BookingCalendarContext : DbContext
    {
        public BookingCalendarContext(DbContextOptions<BookingCalendarContext> options) : base(options) { }

        public DbSet<Floor> Floors { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<TileAssignment> TileAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Floor>()
                .HasMany(f => f.Rooms)
                .WithOne()
                .HasForeignKey(r => r.FloorId)
                .HasPrincipalKey(r => r.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Session>()
                .HasKey(nameof(Session.Id), nameof(Session.TileId));
        }
    }
}
