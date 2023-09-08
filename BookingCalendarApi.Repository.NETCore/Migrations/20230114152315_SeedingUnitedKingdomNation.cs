using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Repository.NETCore.Migrations
{
    public partial class SeedingUnitedKingdomNation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nations",
                columns: new[] { "Iso", "Code", "Description" },
                values: new object[] { "UK", 100000219ul, "REGNO UNITO" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UK");
        }
    }
}
