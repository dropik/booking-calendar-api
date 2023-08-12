using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Migrations
{
    public partial class OriginalGermanyAndUSNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DE",
                column: "Description",
                value: "GERMANIA");

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UM",
                column: "Description",
                value: "STATI UNITI D'AMERICA");

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "US",
                column: "Description",
                value: "STATI UNITI D'AMERICA");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "DE",
                column: "Description",
                value: "GERMANIA - REP");

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "UM",
                column: "Description",
                value: "U.S.A.");

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "US",
                column: "Description",
                value: "U.S.A.");
        }
    }
}
