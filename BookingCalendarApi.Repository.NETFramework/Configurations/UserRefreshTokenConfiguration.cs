using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class UserRefreshTokenConfiguration : EntityTypeConfiguration<UserRefreshToken>
    {
        public UserRefreshTokenConfiguration()
        {
            HasKey(u => u.RefreshToken);

            Property(u => u.Username).IsRequired();
            Property(u => u.ExpiresAt).IsRequired();
        }
    }
}
