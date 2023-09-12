using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class ColorAssignmentConfiguration : EntityTypeConfiguration<ColorAssignment>
    {
        public ColorAssignmentConfiguration()
        {
            ToTable("color_assignments");

            HasKey(a => a.BookingId);
            Property(a => a.Color).IsRequired();
        }
    }
}
