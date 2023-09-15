using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class UserRefreshTokenConfiguration : IEntityTypeConfiguration<UserRefreshToken>
    {
        public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.RefreshToken).IsRequired().HasColumnType($"text");
            builder.Property(u => u.Username).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);
            builder.Property(u => u.ExpiresAt).IsRequired();
        }
    }
}
