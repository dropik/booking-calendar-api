using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookingCalendarApi.Services
{
    public class ColorAssignmentsService : IColorAssignmentsService
    {
        private readonly BookingCalendarContext _context;

        public ColorAssignmentsService(BookingCalendarContext context)
        {
            _context = context;
        }

        public async Task AssignColors(IDictionary<string, string> colors)
        {
            if (colors == null)
            {
                return;
            }

            foreach (var (bookingId, color) in colors)
            {
                var assignment = await _context.ColorAssignments.SingleOrDefaultAsync(a => a.BookingId == bookingId);
                if (assignment != null && assignment.BookingId == bookingId)
                {
                    assignment.Color = color;
                }
                else
                {
                    _context.ColorAssignments.Add(new() { BookingId = bookingId, Color = color });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
