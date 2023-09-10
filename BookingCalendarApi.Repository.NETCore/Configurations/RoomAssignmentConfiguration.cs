using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.NETCore.Configurations
{
    public class RoomAssignmentConfiguration : IEntityTypeConfiguration<RoomAssignment>
    {
        public void Configure(EntityTypeBuilder<RoomAssignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder
                .HasOne(a => a.Room)
                .WithMany()
                .HasForeignKey(a => a.RoomId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
