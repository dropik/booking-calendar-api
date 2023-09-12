using BookingCalendarApi.Repository.Common;
using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            ToTable("users");

            HasKey(u => u.Id);

            Property(u => u.StructureId).IsRequired();
            Property(u => u.Username).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);
            Property(u => u.PasswordHash).IsRequired();
            Property(u => u.VisibleName).HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);

            HasRequired(u => u.Structure).WithMany().HasForeignKey(u => u.StructureId).WillCascadeOnDelete();

            HasIndex(u => u.Username).IsUnique();
        }
    }
}
