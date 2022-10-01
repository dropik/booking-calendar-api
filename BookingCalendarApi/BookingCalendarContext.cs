using BookingCalendarApi.Models;
using System.Data.Entity;

namespace BookingCalendarApi
{
    public class BookingCalendarContext : DbContext
    {
        public DbSet<Floor> Floors { get; set; }
        public DbSet<Room> Rooms { get; set; }

    }
}
