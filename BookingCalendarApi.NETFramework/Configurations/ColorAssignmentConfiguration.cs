using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.Configurations
{
    public class ColorAssignmentConfiguration : IEntityTypeConfiguration<ColorAssignment>
    {
        public void Configure(EntityTypeBuilder<ColorAssignment> builder)
        {
            builder.HasKey(a => a.BookingId);

            builder.Property(a => a.BookingId).HasMaxLength(PropertyDefaults.MAX_ID_LENGTH);
            builder.Property(a => a.StructureId).IsRequired().HasDefaultValue(1);
            builder.Property(a => a.Color).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);

            builder.HasOne(a => a.Structure).WithMany().HasForeignKey(a => a.StructureId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
