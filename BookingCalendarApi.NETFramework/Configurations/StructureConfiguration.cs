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

            builder.Property(s => s.IperbookingHotel).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_CODE_LENGTH);
            builder.Property(s => s.IperbookingUsername).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);
            builder.Property(s => s.IperbookingPassword).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_PASSWORD_LENGTH);

            builder.Property(s => s.ASUtente).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);
            builder.Property(s => s.ASPassword).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_PASSWORD_LENGTH);
            builder.Property(s => s.ASWsKey).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_PASSWORD_HASH_LENGTH);

            builder.Property(s => s.C59Username).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);
            builder.Property(s => s.C59Password).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_PASSWORD_LENGTH);
            builder.Property(s => s.C59Struttura).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_CODE_LENGTH);

            builder.HasData(new Structure() { Id = Structure.MASTER_ID, Name = "Master Hotel" });
        }
    }
}
