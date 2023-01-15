using BookingCalendarApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.ModelConfigurations
{
    public class SessionEntryConfiguration : IEntityTypeConfiguration<SessionEntry>
    {
        public void Configure(EntityTypeBuilder<SessionEntry> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Data).IsRequired();
        }
    }
}
