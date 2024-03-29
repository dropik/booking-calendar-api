﻿using BookingCalendarApi.Repository.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookingCalendarApi.Repository.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.StructureId).IsRequired().HasDefaultValue(1);
            builder.Property(r => r.Number).IsRequired().HasMaxLength(PropertyDefaults.MAX_ROOM_NUMBER_LENGTH);
            builder.Property(r => r.Type).IsRequired().HasMaxLength(PropertyDefaults.MAX_NAME_LENGTH);

            builder.HasOne(r => r.Structure).WithMany().HasForeignKey(r => r.StructureId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
