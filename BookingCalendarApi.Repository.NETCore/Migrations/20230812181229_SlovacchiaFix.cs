using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Repository.NETCore.Migrations
{
    public partial class SlovacchiaFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SK",
                column: "Description",
                value: "REPUBBLICA SLOVACCA");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "SK",
                column: "Description",
                value: "SIRIA");
        }
    }
}
