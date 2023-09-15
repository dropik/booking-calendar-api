using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.Configurations
{
    public class StructureConfiguration : IEntityTypeConfiguration<Structure>
    {
        public void Configure(EntityTypeBuilder<Structure> builder)
        {
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Name).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);

            builder.HasData(new Structure() { Id = Structure.MASTER_ID, Name = "Master Hotel" });
        }
    }
}
