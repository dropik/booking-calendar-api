using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class RoomAssignmentConfiguration : IEntityTypeConfiguration<RoomAssignment>
    {
        public void Configure(EntityTypeBuilder<RoomAssignment> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasMaxLength(PropertyDefaults.MAX_ID_LENGTH);
            builder.HasOne(a => a.Room).WithMany().HasForeignKey(a => a.RoomId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
