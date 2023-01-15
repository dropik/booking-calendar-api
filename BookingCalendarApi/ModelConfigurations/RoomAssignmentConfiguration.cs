using BookingCalendarApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.ModelConfigurations
{
    public class RoomAssignmentConfiguration : IEntityTypeConfiguration<RoomAssignment>
    {
        public void Configure(EntityTypeBuilder<RoomAssignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder
                .HasOne<Room>()
                .WithMany()
                .HasForeignKey(a => a.RoomId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
