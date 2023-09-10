using BookingCalendarApi.Repository.Common;
using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class StructureConfiguration : EntityTypeConfiguration<Structure>
    {
        public StructureConfiguration()
        {
            HasKey(s => s.Id);
            Property(s => s.Name).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);
        }
    }
}
