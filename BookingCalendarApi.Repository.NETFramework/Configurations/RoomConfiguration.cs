using System.Data.Entity.ModelConfiguration;

namespace BookingCalendarApi.Repository.NETFramework.Configurations
{
    public class RoomConfiguration : EntityTypeConfiguration<Room>
    {
        public RoomConfiguration()
        {
            ToTable("rooms");

            HasKey(r => r.Id);

            Property(r => r.Number).IsRequired();
            Property(r => r.Type).IsRequired();
        }
    }
}
