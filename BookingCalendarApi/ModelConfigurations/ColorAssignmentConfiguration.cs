using BookingCalendarApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.ModelConfigurations
{
    public class ColorAssignmentConfiguration : IEntityTypeConfiguration<ColorAssignment>
    {
        public void Configure(EntityTypeBuilder<ColorAssignment> builder)
        {
            builder.HasKey(a => a.BookingId);
            builder.Property(a => a.Color).IsRequired();
        }
    }
}
