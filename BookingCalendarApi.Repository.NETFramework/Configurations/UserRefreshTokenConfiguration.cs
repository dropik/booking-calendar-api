using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class UserRefreshTokenConfiguration : EntityTypeConfiguration<UserRefreshToken>
    {
        public UserRefreshTokenConfiguration()
        {
            ToTable("user_refresh_tokens");

            HasKey(u => u.RefreshToken);

            Property(u => u.RefreshToken).HasMaxLength(256);
            Property(u => u.Username).IsRequired();
            Property(u => u.ExpiresAt).IsRequired();
        }
    }
}
