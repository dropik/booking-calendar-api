using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookingCalendarApi.Repository.NETCore.Migrations
{
    public partial class AddPakistan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MK",
                column: "Description",
                value: "MACEDONIA (EX REPUBBLICA JUGOSLAVA)");

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PS",
                column: "Description",
                value: "PAKISTAN");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "MK",
                column: "Description",
                value: "MACEDONIA(EX REPUBBLICA JUGOSLAVA)");

            migrationBuilder.UpdateData(
                table: "Nations",
                keyColumn: "Iso",
                keyValue: "PS",
                column: "Description",
                value: "ISRAELE");
        }
    }
}
