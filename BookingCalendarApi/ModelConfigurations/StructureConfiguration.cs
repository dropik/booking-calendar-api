using BookingCalendarApi.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.ModelConfigurations
{
    public class StructureConfiguration : IEntityTypeConfiguration<Structure>
    {
        public void Configure(EntityTypeBuilder<Structure> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);
        }
    }
}
