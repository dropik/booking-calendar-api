using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.Configurations
{
    public class RoomAssignmentConfiguration : IEntityTypeConfiguration<RoomAssignment>
    {
        public void Configure(EntityTypeBuilder<RoomAssignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id).HasMaxLength(PropertyDefaults.MAX_ID_LENGTH);
            builder.Property(a => a.StructureId).IsRequired().HasDefaultValue(1);

            builder.HasOne(a => a.Structure).WithMany().HasForeignKey(a => a.StructureId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(a => a.Room).WithMany().HasForeignKey(a => a.RoomId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
