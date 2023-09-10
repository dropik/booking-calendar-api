using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class RoomAssignmentConfiguration : EntityTypeConfiguration<RoomAssignment>
    {
        public RoomAssignmentConfiguration()
        {
            HasKey(a => a.Id);
            HasRequired(a => a.Room).WithMany().HasForeignKey(a => a.RoomId).WillCascadeOnDelete();
        }
    }
}
