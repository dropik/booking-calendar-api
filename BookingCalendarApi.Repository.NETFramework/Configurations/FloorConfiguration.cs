using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class FloorConfiguration : EntityTypeConfiguration<Floor>
    {
        public FloorConfiguration()
        {
            HasKey(f => f.Id);
            Property(f => f.Name).IsRequired();
            HasMany(f => f.Rooms).WithRequired().HasForeignKey(r => r.FloorId).WillCascadeOnDelete();
        }
    }
}
