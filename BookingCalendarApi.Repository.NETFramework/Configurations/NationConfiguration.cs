﻿using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class NationConfiguration : EntityTypeConfiguration<Nation>
    {
        public NationConfiguration()
        {
            HasKey(n => n.Iso);
            Property(n => n.Code).IsRequired();
            Property(n => n.Description).IsRequired();
        }
    }
}
