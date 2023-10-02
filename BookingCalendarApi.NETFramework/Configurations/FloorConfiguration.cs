using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.Configurations
{
    public class FloorConfiguration : IEntityTypeConfiguration<Floor>
    {
        public void Configure(EntityTypeBuilder<Floor> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.StructureId).IsRequired().HasDefaultValue(1);
            builder.Property(f => f.Name).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);

            builder.HasOne(f => f.Structure).WithMany().HasForeignKey(f => f.StructureId).OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(f => f.Rooms).WithOne().HasForeignKey(r => r.FloorId).IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}
