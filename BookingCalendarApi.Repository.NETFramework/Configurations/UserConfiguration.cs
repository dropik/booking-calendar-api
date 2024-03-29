﻿using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.StructureId).IsRequired();
            builder.Property(u => u.Username).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.VisibleName).IsRequired(false).HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);

            builder.HasOne(u => u.Structure).WithMany().HasForeignKey(u => u.StructureId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(u => u.Username).IsUnique();
        }
    }
}
