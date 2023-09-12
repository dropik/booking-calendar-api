using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
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
