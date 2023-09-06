using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.ModelConfigurations
{
    public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasKey(u => u.RefreshToken);

            builder.Property(u => u.Username).IsRequired();
            builder.Property(u => u.ExpiresAt).IsRequired();
        }
    }
}
