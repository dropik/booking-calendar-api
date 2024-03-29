﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class FloorConfiguration : IEntityTypeConfiguration<Floor>
    {
        public void Configure(EntityTypeBuilder<Floor> builder)
        {
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Name).IsRequired();

            builder
                .HasMany(f => f.Rooms)
                .WithOne()
                .HasForeignKey(r => r.FloorId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
